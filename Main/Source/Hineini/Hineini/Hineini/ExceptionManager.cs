using System;
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
                    else if (exceptionMessage.Equals(Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE)) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE, Constants.MessageType.Info);
                        errorHandled = true;
                    }
                    else if (exceptionMessage.Contains("Place can't be identified")) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.WORKING_TO_IDENTIFY_LOCATION, Constants.MessageType.Info);
                        errorHandled = true;
                    }
                }
                if (!errorHandled) {
                    MessagesForm.AddMessage(DateTime.Now, "HEE: " + MainUtility.GetExceptionMessage(e), Constants.MessageType.Error);
                    throw e;
                }
            }
        }
    }
}