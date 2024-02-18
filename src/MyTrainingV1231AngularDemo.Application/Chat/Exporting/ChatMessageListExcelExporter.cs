using System.Collections.Generic;
using System.Linq;
using Abp;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using MyTrainingV1231AngularDemo.Chat.Dto;
using MyTrainingV1231AngularDemo.DataExporting.Excel.MiniExcel;
using MyTrainingV1231AngularDemo.Dto;
using MyTrainingV1231AngularDemo.Storage;

namespace MyTrainingV1231AngularDemo.Chat.Exporting
{
    public class ChatMessageListExcelExporter : MiniExcelExcelExporterBase, IChatMessageListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ChatMessageListExcelExporter(
            ITempFileCacheManager tempFileCacheManager,
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession
            ) : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages)
        {
            var tenancyName = messages.Count > 0 ? messages.First().TargetTenantName : L("Anonymous");
            var userName = messages.Count > 0 ? messages.First().TargetUserName : L("Anonymous");

            var items = new List<Dictionary<string, object>>();

            foreach (var message in messages)
            {
                items.Add(new Dictionary<string, object>()
                {
                    {L("ChatMessage_From"), message.Side == ChatSide.Receiver ? (message.TargetTenantName + "/" + message.TargetUserName) : L("You")},
                    {L("ChatMessage_To"), message.Side == ChatSide.Receiver ? L("You") : (message.TargetTenantName + "/" + message.TargetUserName)},
                    {L("Message"), message.Message},
                    {L("ReadState"), message.Side == ChatSide.Receiver ? message.ReadState : message.ReceiverReadState},
                    {L("CreationTime"), _timeZoneConverter.Convert(message.CreationTime, user.TenantId, user.UserId)},
                });
            }

            return CreateExcelPackage($"Chat_{tenancyName}_{userName}.xlsx", items);
        }
    }
}
