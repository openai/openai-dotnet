import { EmitContext } from "@typespec/compiler";
import { UsageFlags } from "@azure-tools/typespec-client-generator-core";

import {
    emitCodeModel,
    CodeModel,
    CSharpEmitterOptions
} from "@typespec/http-client-csharp";

export async function $onEmit(context: EmitContext<CSharpEmitterOptions>) {
    context.options["generator-name"] = "OpenAILibraryGenerator";
    context.options["emitter-extension-path"] = import.meta.url;

    const [, diagnostics] = await emitCodeModel(context, (model: CodeModel) => {
        for (const m of model.models) {
            if (m.usage & UsageFlags.MultipartFormData) {
                m.usage = m.usage | UsageFlags.Json;
            }
        }
        return model;
    });

    if (diagnostics.length > 0) {
        context.program.reportDiagnostics(diagnostics);
    }
}