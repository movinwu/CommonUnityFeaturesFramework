using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ExampleÊý¾Ý
    /// </summary>
    public class DT_Example : DataRow
    {
        public override int ID { get => id; }

        /// <summary>
        /// id
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// byteÊ¾Àý×Ö¶Î
        /// </summary>
        public sbyte byte_example;

        public override void FromBinary(BinaryReader br)
        {
            id = br.ReadInt32();
            byte_example = br.ReadSByte();
        }
    }
}
