using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_API.Module
{
    public class CCModules
    {
        public static void CorrectText()
        {
            AutoCorrectText.Correct();
        }

        public static void CorrectFolder()
        {
            AutoCorrectText.CorrectFolder();
        }
    }
}
