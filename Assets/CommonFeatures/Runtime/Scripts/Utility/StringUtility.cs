using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonFeatures.Utility
{
    /// <summary>
    /// �ַ�������
    /// <para>ʵ�ּ�Ψһid����</para>
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// ��׼����б��
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StandardBackslash(string str)
        {
            if (str.Contains('\\'))
            {
                var sb = new StringBuilder();

                //������׼����б��
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '\\')
                    {
                        //��б�ܺ������ַ�
                        if (i + 1 < str.Length)
                        {
                            //����
                            if (str[i + 1] == 'a')
                            {
                                sb.Append('\a');
                                i++;
                            }
                            //�˸�
                            else if (str[i + 1] == 'b')
                            {
                                sb.Append('\b');
                                i++;
                            }
                            //��ҳ
                            else if (str[i + 1] == 'f')
                            {
                                sb.Append('\f');
                                i++;
                            }
                            //����
                            else if (str[i + 1] == 'n')
                            {
                                sb.Append('\n');
                                i++;
                            }
                            //�س�
                            else if (str[i + 1] == 'r')
                            {
                                sb.Append('\r');
                                i++;
                            }
                            //ˮƽ�Ʊ�
                            else if (str[i + 1] == 'v')
                            {
                                sb.Append('\v');
                                i++;
                            }
                            //ˮƽ�Ʊ�
                            else if (str[i + 1] == 'v')
                            {
                                sb.Append('\v');
                                i++;
                            }
                            //������
                            else if (str[i + 1] == '\'')
                            {
                                sb.Append('\'');
                                i++;
                            }
                            //˫����
                            else if (str[i + 1] == '\"')
                            {
                                sb.Append('\"');
                                i++;
                            }
                            //��б��
                            else if (str[i + 1] == '\\')
                            {
                                sb.Append('\\');
                                i++;
                            }
                            //������ʶ��Ϊ��б���ַ�
                            else
                            {
                                sb.Append('\\');
                            }
                        }
                        //��б�ܺ���û���ַ�
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
