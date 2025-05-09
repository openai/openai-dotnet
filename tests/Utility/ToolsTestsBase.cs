
using System;
using System.Threading.Tasks;
using OpenAI.Agents;

namespace OpenAI.Tests.Utility;

public class ToolsTestsBase
{
    internal class TestTools
    {
        public static string Echo(string message) => message;
        public static int Add(int a, int b) => a + b;
        public static double Multiply(double x, double y) => x * y;
        public static bool IsGreaterThan(long value1, long value2) => value1 > value2;
        public static float Divide(float numerator, float denominator) => numerator / denominator;
        public static string ConcatWithBool(string text, bool flag) => $"{text}:{flag}";
    }

    internal class TestToolsAsync
    {
        public static async Task<string> EchoAsync(string message)
        {
            await Task.Delay(1); // Simulate async work
            return message;
        }

        public static async Task<int> AddAsync(int a, int b)
        {
            await Task.Delay(1); // Simulate async work
            return a + b;
        }

        public static async Task<double> MultiplyAsync(double x, double y)
        {
            await Task.Delay(1); // Simulate async work
            return x * y;
        }

        public static async Task<bool> IsGreaterThanAsync(long value1, long value2)
        {
            await Task.Delay(1); // Simulate async work
            return value1 > value2;
        }

        public static async Task<float> DivideAsync(float numerator, float denominator)
        {
            await Task.Delay(1); // Simulate async work
            return numerator / denominator;
        }

        public static async Task<string> ConcatWithBoolAsync(string text, bool flag)
        {
            await Task.Delay(1); // Simulate async work
            return $"{text}:{flag}";
        }
    }

    internal class TestMcpClient : McpClient
    {
        private readonly BinaryData _toolsResponse;
        private readonly Func<string, BinaryData> _toolCallResponseFactory;
        private bool _isStarted;

        public TestMcpClient(Uri endpoint, BinaryData toolsResponse, Func<string, BinaryData> toolCallResponseFactory = null)
            : base(endpoint)
        {
            _toolsResponse = toolsResponse;
            _toolCallResponseFactory = toolCallResponseFactory ?? (_ => BinaryData.FromString("\"test result\""));
        }

        public override Task StartAsync()
        {
            _isStarted = true;
            return Task.CompletedTask;
        }

        public override Task<BinaryData> ListToolsAsync()
        {
            if (!_isStarted)
                throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

            return Task.FromResult(_toolsResponse);
        }

        public override Task<BinaryData> CallToolAsync(string toolName, BinaryData parameters)
        {
            if (!_isStarted)
                throw new InvalidOperationException("Session is not initialized. Call StartAsync() first.");

            return Task.FromResult(_toolCallResponseFactory(toolName));
        }
    }
}