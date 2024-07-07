//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © 2024 movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2024/7/6 16:57:28
//------------------------------------------------------------

using CommonFeatures.DataTable;
using System.IO;

namespace HotfixScripts
{
    /// <summary>
    /// Example数据
    /// </summary>
    public class DR_Example : DataRow
    {
        public override int ID { get => id; }

        /// <summary>
        /// id
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// byte示例字段
        /// </summary>
        public sbyte byte_example { get; private set; }

        /// <summary>
        /// short示例字段
        /// </summary>
        public short short_example { get; private set; }

        /// <summary>
        /// int示例字段
        /// </summary>
        public int int_example { get; private set; }

        /// <summary>
        /// long示例字段
        /// </summary>
        public long long_example { get; private set; }

        /// <summary>
        /// string示例字段
        /// </summary>
        public string string_example { get; private set; }

        /// <summary>
        /// bool示例字段
        /// </summary>
        public bool bool_example { get; private set; }

        /// <summary>
        /// float示例字段
        /// </summary>
        public float float_example { get; private set; }

        /// <summary>
        /// double示例字段
        /// </summary>
        public double double_example { get; private set; }

        /// <summary>
        /// char示例字段
        /// </summary>
        public char char_example { get; private set; }

        /// <summary>
        /// 一维数组示例字段
        /// </summary>
        public int[] array_example { get; private set; }

        /// <summary>
        /// 二维数组示例字段
        /// </summary>
        public int[][] array_array_example { get; private set; }

        public override void FromBinary(BinaryReader br)
        {
            this.id = br.ReadInt32();
            this.byte_example = (sbyte)br.ReadByte();
            this.short_example = br.ReadInt16();
            this.int_example = br.ReadInt32();
            this.long_example = br.ReadInt64();
            this.string_example = br.ReadString();
            this.bool_example = br.ReadBoolean();
            this.float_example = br.ReadSingle();
            this.double_example = br.ReadDouble();
            this.char_example = br.ReadChar();
            this.array_example = new int[br.ReadInt32()];
            for (int i = 0; i < this.array_example.Length; i++)
            {
                this.array_example[i] = br.ReadInt32();
            };
            this.array_array_example = new int[br.ReadInt32()][];
            for (int j = 0; j < this.array_array_example.Length; j++)
            {
                this.array_array_example[j] = new int[br.ReadInt32()];
                for (int i = 0; i < this.array_array_example[j].Length; i++)
                {
                    this.array_array_example[j][i] = br.ReadInt32();
                };
            };
        }
    }
}
