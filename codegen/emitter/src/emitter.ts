import { EmitContext } from "@typespec/compiler";

import {
    emitCodeModel,
    CodeModel,
    CSharpEmitterOptions
} from "@typespec/http-client-csharp";

export async function $onEmit(context: EmitContext<CSharpEmitterOptions>) {
    context.options["generator-name"] = "OpenAILibraryGenerator";
    context.options["emitter-extension-path"] = import.meta.url;

    const [, diagnostics] = await emitCodeModel(context);

    if (diagnostics.length > 0) {
        context.program.reportDiagnostics(diagnostics);
    }
}