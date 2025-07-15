import { EmitContext } from "@typespec/compiler";

import {
    $onEmit as $OnMGCEmit,
    CSharpEmitterOptions
} from "@typespec/http-client-csharp";

export async function $onEmit(context: EmitContext<CSharpEmitterOptions>) {
    context.options["generator-name"] = "OpenAILibraryGenerator";
    context.options["emitter-extension-path"] = import.meta.url;
    await $OnMGCEmit(context);
}