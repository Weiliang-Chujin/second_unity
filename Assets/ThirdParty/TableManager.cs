using System;
using System.Collections.Generic;
using System.IO;

namespace TableConfig
{
    public interface ITableModel
    {
        Dictionary<string, string> ParsePerValue { set; get; }
        object Key();
    }

    public abstract class BaseModel : ITableModel
    {
        private Dictionary<string, string> parsePerValue = new Dictionary<string, string>();

        public Dictionary<string, string> ParsePerValue
        {
            get { return parsePerValue; }
            set { parsePerValue = value; }
        }

        public abstract object Key();
    }

    /// <summary>
    /// 管理表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITable2Data<T> where T : BaseModel
    {
        void ReloadTable();
        T GetModel(object key);
        List<T> GetAllModel();
        List<T> GetAllModel(Func<T, bool> comp);
    }

    /// <typeparam name="T">Table</typeparam>
    public sealed class TableManager<T> : ITable2Data<T> where T : BaseModel
    {
        private T[] mModelArray;
        private Dictionary<object, int> mKeyModelDict;

        // specify file
        private string file;

        public TableManager()
        {
            ReloadTable();
        }

        public TableManager(Dictionary<string, string> a)
        {
            ReloadTable();
        }

        public TableManager(string file)
        {
            this.file = file;
            ReloadTable();
        }

        public void ReloadTable()
        {
            if (File.Exists(file))
            {
                TableParser<T>.ParseLocalFile(file, mModelArray =>
                {
                    // end
                    OnParseComplete(mModelArray);
                });
            }
            else
            {
                TableParser<T>.Parse(typeof(T).Name, mModelArray =>
                {
                    // end
                    OnParseComplete(mModelArray);
                });
            }
        }

        private void OnParseComplete(T[] mModelArray)
        {
            this.mModelArray = mModelArray;
            if (mKeyModelDict == null)
                mKeyModelDict = new Dictionary<object, int>();
            else
                mKeyModelDict.Clear();

            for (int i = 0; i < mModelArray.Length; i++)
                mKeyModelDict[mModelArray[i].Key()] = i;
        }

        public T GetModel(object key)
        {
            int index;
            if (mKeyModelDict.TryGetValue(key, out index))
                return mModelArray[index];
            return default(T);
        }

        public List<T> GetAllModel()
        {
            return GetAllModel((m) => true);
        }

        public List<T> GetAllModel(Func<T, bool> comp)
        {
            List<T> list = new List<T>();

            foreach (var t in mModelArray)
            {
                if (comp(t))
                    list.Add(t);
            }

            return list;
        }
    }
}