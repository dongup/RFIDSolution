using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RFIDSolution.WebAdmin.Utils
{
    public static class ModelUtils
    {
        /// <summary>
        /// Copy value from this to child
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public static void CopyProperty<TParent, TChild>(TParent parent, TChild child)
        {
            if (parent == null || child == null) { return; }

            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        try
                        {
                            childProperty.SetValue(child, parentProperty.GetValue(parent));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                        }
                        break;
                    }
                }
            }
        }

        public static void CopyPropertyJson<TParent, TChild>(TParent parent, TChild child)
        {
            string json = JsonConvert.SerializeObject(parent);
            var newChild = JsonConvert.DeserializeObject<TChild>(json);
            CopyProperty(newChild, child);
        }
    }
}
