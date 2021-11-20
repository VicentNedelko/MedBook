using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Models.Enums
{
    public static class EnumConverter
    {
        public static string TypeToString(IndTYPE indTYPE)
        {
            return indTYPE switch
            {
                IndTYPE.VALUE => "Цифр. значение",
                IndTYPE.YESNO => "Обнар./Не обнар.",
                _ => "Не определено"
            };
        }

        public static IndTYPE IntToEnum(int type)
        {
            return type switch
            {
                0 => IndTYPE.VALUE,
                1 => IndTYPE.YESNO,
                _ => IndTYPE.VALUE,
            };
        }
    }
}
