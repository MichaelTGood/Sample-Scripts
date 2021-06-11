using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlayFab.Plugins.CloudScript;
using PlayFab;
using System;

namespace Kkachi
{
    public static class VerifyData
    {
        /// <summary> Verifies the provided data version from the user against the latest version on the server. 
        /// </summary>
        /// <returns> True, if data refresh is required, as well as necessary Shared Access Signatures
        /// </returns>
        [FunctionName("VerifyData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            Binder binder,
            ILogger log)
        {
            log.LogInformation("VerifyData has been requested to Execute");

            ////////////////////////////////////////////////////////////////
            
            ///-- Declarations 
            var context = await FunctionContext<DataRequest>.Create(req);
            var request = context.FunctionArgument;
            var verifyDataResponse = new VerifyDataResponse();
            var serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            string gameEvent = request.EventCode;
            string logFile = $"{Constants.BlobStorage.EVENTS}/{gameEvent}/VerifyDataLog";
            string eventDataFileLocation = $"{Constants.BlobStorage.EVENTS}/{gameEvent}/{gameEvent}.json";
            string eventDataFile_Raw = null;

            ////////////////////////////////////////////////////////////////

            //-- Fetch the latest EventDataFile
            using (var reader = await binder.BindAsync<TextReader>(new BlobAttribute(eventDataFileLocation, FileAccess.Read)))
                { eventDataFile_Raw = reader.ReadToEnd(); }


            //-- Deserialize the EventDataFile Revision information
            var eventDataFile_Limited = serializer.DeserializeObject<EventDataFile_Revision>(eventDataFile_Raw);


            //-- Get the SAS File
            string sasFileLocation = $"secure/SAS.json";
            string sasFile;
            using (var reader = await binder.BindAsync<TextReader>(new BlobAttribute(sasFileLocation, FileAccess.Read)))
                { sasFile = reader.ReadToEnd(); }
            

            //-- Extract this event's SAS keys from the SAS File.
            var sasList = serializer.DeserializeObject<dynamic>(sasFile);
            if(!sasList.TryGetValue(gameEvent, out object eventsas))
            {
                //-- Log Error and return to user
                LogLineBuilder(out string e_logLine, out string e_terminalLine, false, gameEvent, context, "COULD NOT FIND GAME EVENT IN SAS FILE");
                log.LogError(e_terminalLine);

                using(var writer = binder.Bind<TextWriter>(new BlobAttribute(logFile)))
                    { writer.WriteLine(e_logLine); }

                return null;
            }

            var finalEventSAS = serializer.DeserializeObject<EventSAS>(eventsas.ToString());
            

            //-- Prepare Verify Return
            if(request.Revision < eventDataFile_Limited.Revision)
            {
                verifyDataResponse.DataRefreshRequired = true;
                verifyDataResponse.EDF_SAS = finalEventSAS.EDF;
            }
            else
            { 
                verifyDataResponse.DataRefreshRequired = false; 
            }

            verifyDataResponse.Images_SAS = finalEventSAS.Images;

            ////////////////////////////////////////////////////////////////
            
            //-- Log Success
            LogLineBuilder(out string logLine, out string terminalLine, true, gameEvent, context);
            log.LogInformation(terminalLine);
            
            using (var writer = binder.Bind<TextWriter>(new BlobAttribute(logFile)))
                { writer.WriteLine(logLine); }

            //-- Return Revision an SAS codes
            return new OkObjectResult(serializer.SerializeObject(verifyDataResponse));
        }



        /// <summary> Compiles a Log Entry.
        /// </summary>
        private static void LogLineBuilder(out string logLine, out string terminalLine, 
                                            bool success, string gameEvent, FunctionContext<DataRequest> context, string message = null)
        {
            string logLine_temp = DateTime.UtcNow.ToLongTimeString() + ',';

            if(success)
                { logLine_temp += "SUCCESS,"; }
            else
                { logLine_temp += "ERROR,"; }
            
            logLine_temp += $"{gameEvent ?? "NO EVENT CODE"},";

            logLine_temp += $"{context.CallerEntityProfile.Lineage.TitlePlayerAccountId ?? "NO TITLE PLAYER ID"},";

            logLine_temp += $"{context.FunctionArgument.PlayFabId ?? "NO MASTER PLAYER ID"},";

            logLine_temp += $"{context.CallerEntityProfile.Lineage.TitleId ?? "NO TITLE ID"},";

            logLine_temp += $"{context.FunctionArgument.Revision.ToString() ?? "NO REVISION SENT"},";

            if(!string.IsNullOrEmpty(message))
                { logLine_temp += $"{message},"; }

            logLine = logLine_temp;
            terminalLine = logLine_temp.Replace(",", " | ");
        }
    }

}
