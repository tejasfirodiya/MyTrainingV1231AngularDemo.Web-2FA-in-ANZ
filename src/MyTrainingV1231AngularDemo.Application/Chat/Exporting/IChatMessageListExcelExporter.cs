using System.Collections.Generic;
using Abp;
using MyTrainingV1231AngularDemo.Chat.Dto;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}
