using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hineini.Classes {
    static class AutoSizeLabel {

        // Externals ----------------------

        private struct RECT {

            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

        }

        private const int DT_CALCRECT = 0x400;

        private const int DT_CENTER = 0x1;

        private const int DT_LEFT = 0x0;

        private const int DT_RIGHT = 0x2;

        private const int DT_TOP = 0x0;

        private const int DT_WORDBREAK = 0x10;

        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DeleteObject(IntPtr hObject);
        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DrawText(IntPtr hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);



        public static void AutoSizeLabelHeight(Label ctlLabel) {

            // Auto size the height of a Label control based on the contents of the label.

            // Note: This routine is best called from the Form's Resize event so that changes to screen size and orientation

            // will force the Label height to be adjusted.

            RECT uRECT = default(RECT);

            try {

                // Create a Graphics object. We need a Graphics object so we can get a handle to a Device Context to be used

                // later in the DrawText API. However the Label control in CF2.0 does not support the CreateGraphics method,

                // so create the Graphics object from the form on which the Label is located.

                // Note: This can cause an exception when this routine is called from a Form's Resize event. Probably because

                // the form is not fully initialised and the Resize event may be called one or more times during form

                // initialisation. Therefore, the entire routine is wrapped in a Try/Catch block.

                Graphics objGraphics = ctlLabel.TopLevelControl.CreateGraphics();

                // -------------------------------------------------------------

                // Note: An alternative to the above method of creating a Graphics object is to create a Bitmap object and

                // obtain the Graphics object from it as follows.

                //Dim objBitmap As New Bitmap(1, 1)

                //Dim objGraphics As Graphics = Graphics.FromImage(objBitmap)

                // And remembering to dispose of the Bitmap object in a cleanup at the end of the routine as follows.

                //objBitmap.Dispose()

                // This method would remove the need for the Try/Catch block, but means that Bitmap objects are repeatedly

                // being created and destroyed.

                // -------------------------------------------------------------

                // Get the handle to the Device Context of the Graphics object

                IntPtr hDc = objGraphics.GetHdc();


                {
                    // Get the handle to the Font of the Label

                    IntPtr hFont = ctlLabel.Font.ToHfont();

                    // Apply the Font to the Graphics object

                    IntPtr hFontOld = SelectObject(hDc, hFont);

                    // Set the initial size of the Rect

                    uRECT.Right = ctlLabel.Width;

                    uRECT.Bottom = ctlLabel.Height;

                    // Build the base format

                    int lFormat = DT_CALCRECT | DT_WORDBREAK | DT_TOP;

                    // -------------------------------------------------------------

                    // Adjust the format to the Label's text alignment.

                    // Note: This probably isn't necessary as the horizontal alignment of text shouldn't affect the text

                    // height calculation. But just in case...

                    switch (ctlLabel.TextAlign) {

                        case ContentAlignment.TopLeft:

                            lFormat = lFormat | DT_LEFT;

                            break;
                        case ContentAlignment.TopCenter:

                            lFormat = lFormat | DT_CENTER;

                            break;
                        case ContentAlignment.TopRight:

                            lFormat = lFormat | DT_RIGHT;

                            break;
                    }

                    // -------------------------------------------------------------

                    // Calculate the Rect of the text

                    if (DrawText(hDc, ctlLabel.Text, -1, ref uRECT, lFormat) != 0) {
                        // Success

                        // Apply the new height to the label


                        ctlLabel.Height = uRECT.Bottom;
                    }

                    // Cleanup ----------------------------

                    // Set the Font of the Graphics object back to the original

                    SelectObject(hDc, hFontOld);

                    // Delete the handle to the Font of the Label


                    DeleteObject(hFont);
                }

                // Clean up the Graphics object


                objGraphics.Dispose();
            }
            catch {

            }
            // Do nothing

            //Debug.WriteLine("AutoSizeLabelHeight failed.")


        }

    }
}