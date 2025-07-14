using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Utils
{
    public static class StringFormatting
    {
        /// <summary>
        /// 한글/영문이 섞인 문자열의 시각적 길이를 계산한다. (한글 2, 나머지 1로 처리)
        /// </summary>
        /// <param name="str">길이를 계산할 문자열</param>
        /// <returns>계산된 시각적 길이</returns>
        public static int GetPrintableLength(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            int length = 0;
            foreach (char c in str)
            {
                // 한글 범위(가-힣)에 속하는 문자인지 확인
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2; // 한글은 2칸으로 계산
                }
                else
                {
                    length += 1; // 그 외 (영문, 숫자, 특수문자 등)는 1칸으로 계산
                }
            }

            return length;
        }

        /// <summary>
        /// 주어진 문자열을 지정된 시각적 길이에 맞춰 오른쪽을 공백으로 채운다.
        /// </summary>
        /// <param name="str">패딩을 추가할 문자열</param>
        /// <param name="totalLength">목표로 하는 총 시각적 길이</param>
        /// <returns>패딩이 추가된 문자열</returns>
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str);
            int padding = totalLength - currentLength;
            return str + new string(' ', padding > 0 ? padding : 0);
        }
    }
}
