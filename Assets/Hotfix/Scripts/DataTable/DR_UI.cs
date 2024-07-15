//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © 2024 movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2024/7/14 20:07:09
//------------------------------------------------------------

using CommonFeatures.DataTable;
using System.IO;

namespace HotfixScripts
{
    /// <summary>
    /// UI数据
    /// </summary>
    public class DR_UI : DataRow
    {
        public override int ID { get => id; }

        /// <summary>
        /// id
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// 界面预制体
        /// </summary>
        public string panel_prefab { get; private set; }

        /// <summary>
        /// 是否暂停上层界面
        /// </summary>
        public bool pause_last_panel { get; private set; }

        /// <summary>
        /// 是否引导界面
        /// </summary>
        public bool is_guide { get; private set; }

        public override void FromBinary(BinaryReader br)
        {
            this.id = br.ReadInt32();
            this.panel_prefab = br.ReadString();
            this.pause_last_panel = br.ReadBoolean();
            this.is_guide = br.ReadBoolean();
        }
    }
}
