using System.Text.Json;
using System.Text.Json.Nodes;
using Lagrange.Core;
using Lagrange.Core.Message.Entity;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.OneBot.Core.Entity.Action;
using Lagrange.OneBot.Core.Operation.Converters;
using System;


namespace Lagrange.OneBot.Core.Operation.Generic;

[Operation(".ocr_image")]
[Operation("ocr_image")]
public class OcrImageOperation() : IOperation
{
    public async Task<OneBotResult> HandleOperation(BotContext context, JsonNode? payload)
    {
        if (payload.Deserialize<OneBotOcrImage>(SerializerOptions.DefaultOptions) is { } data)
        {
            if (Uri.TryCreate(data.Image, UriKind.Absolute, out Uri? uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                var res = await context.OcrImage(data.Image);
                return new OneBotResult(res, 0, "ok");
            }
            else
            {
                if (CommonResolver.ResolveStream(data.Image) is { } stream)
                {
                    var entity = new ImageEntity(stream);
                    var res = await context.OcrImage(entity); 
                    return new OneBotResult(res, 0, "ok");
                }
                else
                {
                    throw new Exception("Invalid image data. Must be a valid URL, file path, or Base64 encoded image.");
                }
            }
        }

        throw new Exception("Invalid payload for OCR operation.");
    }
}
