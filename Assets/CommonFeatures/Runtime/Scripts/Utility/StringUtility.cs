using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonFeatures.Utility
{
    /// <summary>
    /// 字符串工具
    /// <para>实现简单唯一id生成</para>
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// 标准化反斜杠
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StandardBackslash(string str)
        {
            if (str.Contains('\\'))
            {
                var sb = new StringBuilder();

                //遍历标准化反斜杠
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '\\')
                    {
                        //反斜杠后面有字符
                        if (i + 1 < str.Length)
                        {
                            //响铃
                            if (str[i + 1] == 'a')
                            {
                                sb.Append('\a');
                                i++;
                            }
                            //退格
                            else if (str[i + 1] == 'b')
                            {
                                sb.Append('\b');
                                i++;
                            }
                            //换页
                            else if (str[i + 1] == 'f')
                            {
                                sb.Append('\f');
                                i++;
                            }
                            //换行
                            else if (str[i + 1] == 'n')
                            {
                                sb.Append('\n');
                                i++;
                            }
                            //回车
                            else if (str[i + 1] == 'r')
                            {
                                sb.Append('\r');
                                i++;
                            }
                            //水平制表
                            else if (str[i + 1] == 'v')
                            {
                                sb.Append('\v');
                                i++;
                            }
                            //水平制表
                            else if (str[i + 1] == 'v')
                            {
                                sb.Append('\v');
                                i++;
                            }
                            //单引号
                            else if (str[i + 1] == '\'')
                            {
                                sb.Append('\'');
                                i++;
                            }
                            //双引号
                            else if (str[i + 1] == '\"')
                            {
                                sb.Append('\"');
                                i++;
                            }
                            //反斜线
                            else if (str[i + 1] == '\\')
                            {
                                sb.Append('\\');
                                i++;
                            }
                            //都不是识别为反斜杠字符
                            else
                            {
                                sb.Append('\\');
                            }
                        }
                        //反斜杠后面没有字符
                        else
                        {
                            sb.Append(str[i]);
                        }
                    }
                    else
                    {
                        sb.Append(str[i]);
                    }
                }

                return sb.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}
