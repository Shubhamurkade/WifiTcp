using System;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace TcpServer
{
    static class WindowProcessor
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowEx",
         CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,
         IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);

        public static void GetWindowElements(IntPtr handle)
        {
            
        }

        public static AutomationElement GetAutomationElementFromHandle(IntPtr handle)
        {
            return AutomationElement.FromHandle(handle);
        }

        public static AutomationElementCollection FindElementsByCondition(AutomationElement element)
        {
            return element.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
        }

        public static int GetAllChildrenWindowHandles(IntPtr hParent, int maxCount)
        {
            while (true)
            {
                int ct = 0;

                IntPtr currChild = IntPtr.Zero;

                while (ct < maxCount)
                {
                    currChild = FindWindowEx(hParent, IntPtr.Zero, null, null);
                    if (currChild == IntPtr.Zero) break;

                    AutomationElement element = WindowProcessor.GetAutomationElementFromHandle(currChild);

                    //Console.WriteLine(element.GetCurrentPropertyValue(AutomationElement.NameProperty) as string);

                    //Console.WriteLine(element.GetCurrentPropertyValue(AutomationElement.IsTextPatternAvailableProperty)) ;
                    AutomationElementCollection colleciton = element.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                    foreach(AutomationElement el in colleciton)
                    {
                        Console.WriteLine(el);
                        InsertTextUsingUIAutomation(element, "abc", out StringBuilder a, out Boolean c);
                        Console.WriteLine(a);
                    }
                    
                    hParent = currChild;
                    ++ct;
                }

            }
            return 1;
        }

        public static void InsertTextUsingUIAutomation(AutomationElement element,
                                    string value, out StringBuilder feedbackText, out Boolean resultStat)
        {
            feedbackText = new StringBuilder();

            try
            {
                // Validate arguments / initial setup
                if (value == null)
                    throw new ArgumentNullException(
                        "String parameter must not be null.");

                if (element == null)
                    throw new ArgumentNullException(
                        "AutomationElement parameter must not be null");

                // A series of basic checks prior to attempting an insertion.
                //
                // Check #1: Is control enabled?
                // An alternative to testing for static or read-only controls 
                // is to filter using 
                // PropertyCondition(AutomationElement.IsEnabledProperty, true) 
                // and exclude all read-only text controls from the collection.
                if (!element.Current.IsEnabled)
                {
                    throw new InvalidOperationException(
                        "The control with an AutomationID of "
                        + element.Current.AutomationId.ToString()
                        + " is not enabled.\n\n");
                }

                // Check #2: Are there styles that prohibit us 
                //           from sending text to this control?
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new InvalidOperationException(
                        "The control with an AutomationID of "
                        + element.Current.AutomationId.ToString()
                        + "is read-only.\n\n");
                }


                // Once you have an instance of an AutomationElement,  
                // check if it supports the ValuePattern pattern.
                object valuePattern = null;

                // Control does not support the ValuePattern pattern 
                // so use keyboard input to insert content.
                //
                // NOTE: Elements that support TextPattern 
                //       do not support ValuePattern and TextPattern
                //       does not support setting the text of 
                //       multi-line edit or document controls.
                //       For this reason, text input must be simulated
                //       using one of the following methods.
                //       
                if (!element.TryGetCurrentPattern(
                    ValuePattern.Pattern, out valuePattern))
                {
                    feedbackText.Append("The control with an AutomationID of ")
                        .Append(element.Current.AutomationId.ToString())
                        .Append(" does not support ValuePattern.")
                        .AppendLine(" Using keyboard input.\n");

                    // Set focus for input functionality and begin.
                    element.SetFocus();

                    // Pause before sending keyboard input.
                    Thread.Sleep(100);

                    // Delete existing content in the control and insert new content.
                    SendKeys.SendWait("^{HOME}");   // Move to start of control
                    SendKeys.SendWait("^+{END}");   // Select everything
                    SendKeys.SendWait("{DEL}");     // Delete selection
                    SendKeys.SendWait(value);
                    resultStat = true;
                }
                // Control supports the ValuePattern pattern so we can 
                // use the SetValue method to insert content.
                else
                {
                    feedbackText.Append("The control with an AutomationID of ")
                        .Append(element.Current.AutomationId.ToString())
                        .Append((" supports ValuePattern."))
                        .AppendLine(" Using ValuePattern.SetValue().\n");

                    // Set focus for input functionality and begin.
                    element.SetFocus();

                    ((ValuePattern)valuePattern).SetValue(value);
                    resultStat = true;
                }
            }
            catch (ArgumentNullException exc)
            {
                feedbackText.Append(exc.Message);
            }
            catch (InvalidOperationException exc)
            {
                feedbackText.Append(exc.Message);
            }

            resultStat = false;
        }
    }
}
