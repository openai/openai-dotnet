using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace OpenAI.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class RecordedTestAttribute : TestAttribute, IWrapSetUpTearDown
{
    public TestCommand Wrap(TestCommand command)
    {
        ITest test = command.Test;
        while (test.Fixture == null && test.Parent != null)
        {
            test = test.Parent;
        }

        if (test.Fixture is Microsoft.ClientModel.TestFramework.RecordedTestBase fixture)
        {
            return new RecordedTestAttributeCommand(command, fixture.Mode);
        }

        return command;
    }

    private class RecordedTestAttributeCommand : DelegatingTestCommand
    {
        private readonly Microsoft.ClientModel.TestFramework.RecordedTestMode _mode;

        public RecordedTestAttributeCommand(
            TestCommand innerCommand,
            Microsoft.ClientModel.TestFramework.RecordedTestMode mode) : base(innerCommand)
        {
            _mode = mode;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            context.CurrentResult = innerCommand.Execute(context);

            if (!IsTestFailed(context))
            {
                return context.CurrentResult;
            }

            if (_mode == Microsoft.ClientModel.TestFramework.RecordedTestMode.Playback)
            {
                string resultMessage = context.CurrentResult.Message;
                TestResult originalResult = context.CurrentResult;

                if (resultMessage?.Contains(typeof(Microsoft.ClientModel.TestFramework.TestRecordingMismatchException).FullName!) ?? false
                    && !IsAutoRecordingDisabled())
                {
                    context.CurrentResult = context.CurrentTest.MakeTestResult();
                    SetRecordMode((context.TestObject as Microsoft.ClientModel.TestFramework.RecordedTestBase)!, Microsoft.ClientModel.TestFramework.RecordedTestMode.Record);
                    context.CurrentResult = innerCommand.Execute(context);

                    if (context.CurrentResult.ResultState.Status == TestStatus.Passed)
                    {
                        string message = "Test failed playback, but was successfully re-recorded. It should pass if re-run."
                            + Environment.NewLine
                            + Environment.NewLine
                            + originalResult.Message;

                        context.CurrentResult.SetResult(ResultState.Error, message);
                    }
                    else
                    {
                        string message = "The [RecordedTest] attribute attempted to re-record, but failed:"
                            + Environment.NewLine
                            + Environment.NewLine
                            + context.CurrentResult.Message
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Original failure:"
                            + Environment.NewLine
                            + Environment.NewLine
                            + originalResult.Message;

                        context.CurrentResult.SetResult(context.CurrentResult.ResultState, message, context.CurrentResult.StackTrace);
                    }

                    SetRecordMode((context.TestObject as Microsoft.ClientModel.TestFramework.RecordedTestBase)!, Microsoft.ClientModel.TestFramework.RecordedTestMode.Playback);
                    return context.CurrentResult;
                }

                if (resultMessage?.Contains(typeof(Microsoft.ClientModel.TestFramework.TestTimeoutException).FullName!) == true)
                {
                    HandleTestTimeout(context, originalResult);
                }
            }

            return context.CurrentResult;
        }

        private static bool IsAutoRecordingDisabled()
        {
            string switchValue =
                TestContext.Parameters["DisableAutoRecording"]
                ?? Environment.GetEnvironmentVariable("CLIENTMODEL_DISABLE_AUTO_RECORDING");

            return bool.TryParse(switchValue, out bool disabled) && disabled;
        }

        private TestResult HandleTestTimeout(TestExecutionContext context, TestResult originalResult)
        {
            List<TestResult> results = [originalResult];

            for (int retryCount = 0; retryCount < 2; retryCount++)
            {
                context.CurrentResult = context.CurrentTest.MakeTestResult();
                context.CurrentResult = innerCommand.Execute(context);
                results.Add(context.CurrentResult);

                if (!IsTestFailed(context))
                {
                    context.CurrentResult.SetResult(
                        ResultState.Success,
                        ConstructRetryMessage("Test timed out initially, but passed on retry", results));
                    return context.CurrentResult;
                }
            }

            context.CurrentResult.SetResult(
                ResultState.Error,
                ConstructRetryMessage("The test timed out on all attempts", results));

            return context.CurrentResult;
        }

        private static string ConstructRetryMessage(string header, IEnumerable<TestResult> results)
        {
            string attemptDetails = string.Join(
                Environment.NewLine,
                System.Linq.Enumerable.Select(results, (r, i) => $"Attempt {i + 1}: {r.Message ?? "Passed"}"));

            return header + ":" + Environment.NewLine + attemptDetails + Environment.NewLine;
        }

        private static void SetRecordMode(
            Microsoft.ClientModel.TestFramework.RecordedTestBase fixture,
            Microsoft.ClientModel.TestFramework.RecordedTestMode mode)
        {
            fixture.Mode = mode;
        }

        private static bool IsTestFailed(TestExecutionContext context)
            => context.CurrentResult.ResultState.Status switch
            {
                TestStatus.Passed => false,
                TestStatus.Skipped => false,
                _ => true,
            };
    }
}
