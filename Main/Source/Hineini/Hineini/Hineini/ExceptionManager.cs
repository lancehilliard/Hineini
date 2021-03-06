using System;
using System.Drawing;
using Hineini.Utility;

namespace Hineini {
    public class ExceptionManager {
        public static void HandleExpectedErrors(Exception e) {
            bool errorHandled = false;
            if (e != null) {
                string exceptionMessage = MainUtility.GetExceptionMessage(e);
                if (Helpers.StringHasValue(exceptionMessage)) {
                    if (exceptionMessage.Contains("EXAMPLE OF TEXT THAT MEANS SUCCESSFUL UPDATE DESPITE EXCEPTION")) {
                        MessagesForm.AddMessage(DateTime.Now, "Location updated (DESPITE EXCEPTION)", Constants.MessageType.Info);
                        errorHandled = true;
                    }
                    else if (exceptionMessage.Contains("Invalid location query")) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_NOT_YET_IDENTIFIED, Constants.MessageType.Info);
                        MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_INVALID, Constants.MessageType.Error);
                        Helpers.WriteToExtraLog(e.Message, e);
                        errorHandled = true;
                    }
                    else if (exceptionMessage.Equals(Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE)) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE, Constants.MessageType.Info);
                        MessagesForm.AddMessage(DateTime.Now, Constants.TOWER_GEOLOCATION_FAILED, Constants.MessageType.Error);
                        errorHandled = true;
                    }
                    else if (exceptionMessage.Contains("Place can't be identified")) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_NOT_YET_IDENTIFIED, Constants.MessageType.Info);
                        MessagesForm.AddMessage(DateTime.Now, Constants.YAHOO_FAILED_GEOLOCATION, Constants.MessageType.Error);
                        //pendingMapImage = null;
                        errorHandled = true;
                    }
                }
                if (!errorHandled) {
                    Helpers.WriteToExtraLog(e.Message, e);
                    throw e;
                }
            }
        }
    }
}