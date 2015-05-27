using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace SurfaceApplication1
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Settings
    {
        private static volatile Settings instance;
        private static object lockObject = new Object();


        private Settings()
        {

        }

        private List<string> loadedKeys = new List<string>();



        private List<IdeaBall> _balls;
        public List<IdeaBall> Balls
        {
            get
            {
                return EnsureLoaded<List<IdeaBall>>(() => _balls, _balls);
            }
            set
            {
                _balls = value;
                SaveAsJson(() => _balls, value);
            }
        }

        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }


        private void SaveAsJson<T>(Expression<Func<T>> property, object data)
        {
            var expression = GetMemberInfo(property);

            string key = expression.Member.Name.ToLower();
            string jsonString = JsonConvert.SerializeObject(data);
            System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), key + ".dat"));
            file.WriteLine(jsonString);
            file.Close();
        }

        private T EnsureLoaded<T>(Expression<Func<T>> property, T data)
        {
            var expression = GetMemberInfo(property);
            string key = expression.Member.Name.ToLower();
            if (!loadedKeys.Contains(key))
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), key + ".dat");
                if (File.Exists(path))
                {
                    string jsonString = System.IO.File.ReadAllText(path);
                    data = JsonConvert.DeserializeObject<T>(jsonString);
                    loadedKeys.Add(key);
                }
            }
            return data;
        }



        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new Settings();
                    }
                }

                return instance;
            }
        }



    }
}