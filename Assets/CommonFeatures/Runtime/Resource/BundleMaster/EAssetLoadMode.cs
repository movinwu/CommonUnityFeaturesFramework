namespace BundleMaster
{
    /// <summary>
    /// ��Դ���ط�ʽö��
    /// </summary>
    public enum EAssetLoadMode : byte
    {
        /// <summary>
        /// �ӱ༭����ֱ�Ӽ���,���ڿ���
        /// </summary>
        Editor,

        /// <summary>
        /// �ӱ���ֱ�Ӷ�ȡAB����Դ,���ڷ��ȸ����
        /// </summary>
        LocalAB,

        /// <summary>
        /// ��Զ�˶�ȡ����AB����Դ,�����ȸ����
        /// </summary>
        RemoteAB,
    }
}
