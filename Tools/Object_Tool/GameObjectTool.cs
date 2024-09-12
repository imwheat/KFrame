using UnityEngine;

namespace KFrame.Tools
{
    /// <summary>
    /// Koo游戏对象工具
    /// </summary>
    public static partial class UtilityTools
    {
        private static readonly Vector3 ZeroVector = Vector3.zero;
        private static readonly Quaternion IdentityQuaternion = Quaternion.identity;


        //Unity设计中注重保持简洁和通用。尽管拓展方法可以方便地扩展现有类的功能，但它们也可能增加代码的复杂性和难以预测性。因此，Unity 通常遵循一些简单且易于理解的设计原则，避免引入过多的拓展方法。

        #region Unity未提供的静态拓展

        #region Instantiate 实例化(静态拓展)

        /// <summary>
        /// 实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Component。</param>
        /// <returns>新实例化的 GameObject；如果目标为空，则返回 null。</returns>
        public static GameObject KooInstantiateSelf(this GameObject target)
        {
            return Object.Instantiate(target);
        }

        /// <summary>
        /// 在指定的父级下实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Transform。</param>
        /// <returns>新实例化的 GameObject。</returns>
        public static GameObject KooInstantiateSelf(this GameObject target, Transform parent,
            bool worldPositionStays = false)
        {
            if (parent is not null)
            {
                return Object.Instantiate(target, parent, worldPositionStays);
            }

            if (target is not null) return Object.Instantiate(target, ZeroVector, IdentityQuaternion);
            Debug.LogWarning("KooGameObjectExHelp:不能实例化空的游戏物体.");
            return null;
        }


        /// <summary>
        /// 在指定的父级下实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Component。</param>
        /// <returns>新实例化的 GameObject；如果目标为空，则返回 null。</returns>
        public static GameObject KooInstantiateSelf(this GameObject target, Component parent)
        {
            Transform parentTransform = parent?.transform;
            return KooInstantiateSelf(target, parentTransform);
        }

        /// <summary>
        /// 在指定的父级下实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Component。</param>
        /// <returns>新实例化的 GameObject；如果目标为空，则返回 null。</returns>
        public static GameObject KooInstantiateSelf(this GameObject target, GameObject parent)
        {
            return KooInstantiateSelf(target, parent?.transform);
        }

        /// <summary>
        /// 在指定的父级下实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Component。</param>
        /// <returns>新实例化的 GameObject；如果目标为空，则返回 null。</returns>
        public static GameObject KooInstantiateSelf(this Component target, Component parent)
        {
            return KooInstantiateSelf(target?.gameObject, parent?.transform);
        }

        /// <summary>
        /// 在指定的父级下实例化目标 GameObject。
        /// </summary>
        /// <param name="target">要实例化的 GameObject。</param>
        /// <param name="parent">新实例的父级 Component。</param>
        /// <returns>新实例化的 GameObject；如果目标为空，则返回 null。</returns>
        public static GameObject KooInstantiateSelf(this Component target, GameObject parent)
        {
            return KooInstantiateSelf(target?.gameObject, parent?.transform);
        }

        #endregion

        #region Destroy 销毁(静态拓展)

        #region DestroyObject

        /// <summary>
        /// 销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        public static void KooDestroyGameObject(this GameObject target)
        {
            if (target == null)
            {
                return;
            }

            Object.Destroy(target);
        }

        /// <summary>
        /// 立即销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        public static void KooDestroyGameObjectImmediate(this GameObject target)
        {
            if (target == null)
            {
                return;
            }

            Object.DestroyImmediate(target);
        }

        /// <summary>
        /// 销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        public static void KooDestroyGameObject(this Component target)
        {
            KooDestroyGameObject(target?.gameObject);
        }

        /// <summary>
        /// 立即销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        public static void KooDestroyGameObjectImmediate(this Component target)
        {
            KooDestroyGameObjectImmediate(target?.gameObject);
        }

        #endregion

        #region DestroyGameObjDelay

        /// <summary>
        /// 延时销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        /// <param name="time">延时时间，以秒为单位。</param>
        public static void KooDestroyGameObjectDelay(this GameObject target, float time)
        {
            if (target == null)
            {
                return;
            }

            Object.Destroy(target, time);
        }

        /// <summary>
        /// 延时销毁目标 GameObject。
        /// </summary>
        /// <param name="target">要销毁的 GameObject。</param>
        /// <param name="time">延时时间，以秒为单位。</param>
        public static void KooDestroyGameObjectDelay(this Component target, float time)
        {
            KooDestroyGameObjectDelay(target?.gameObject, time);
        }

        #endregion

        #region DestroyChildren

        /// <summary>
        /// 销毁子节点
        /// </summary>
        /// <param name="target">要清空子物体的 Transform</param>
        /// <param name="index">需要清空到第几个子节点为止，默认为0，即全部清除</param>
        public static void KooDestroyChildren(this Transform target, int index = 0)
        {
            if (target == null)
            {
                return;
            }

            int len = target.childCount;
            for (int i = len - 1; i >= index; i--)
            {
                Transform child = target.GetChild(i);
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 立即销毁子节点
        /// </summary>
        /// <param name="target">要清空子物体的 Transform</param>
        /// <param name="index">需要清空到第几个子节点为止，默认为0，即全部清除</param>
        public static void KooDestroyChildrenImmediate(this Transform target, int index = 0)
        {
            if (target == null)
            {
                return;
            }

            int len = target.childCount;
            for (int i = len - 1; i >= index; i--)
            {
                Transform child = target.GetChild(i);
                Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// 销毁子节点
        /// </summary>
        /// <param name="target">要清空子物体的 Transform</param>
        /// <param name="index">需要清空到第几个子节点为止，默认为0，即全部清除</param>
        public static void KooDestroyChildren(this Component target, int index = 0)
        {
            KooDestroyChildren(target?.transform, index);
        }

        /// <summary>
        /// 销毁子节点
        /// </summary>
        /// <param name="target">要清空子物体的 Transform</param>
        /// <param name="index">需要清空到第几个子节点为止，默认为0，即全部清除</param>
        public static void KooDestroyChildren(this GameObject target, int index = 0)
        {
            KooDestroyChildren(target?.transform, index);
        }
        
        /// <summary>
        /// 销毁子节点
        /// </summary>
        /// <param name="target">要清空子物体的 Transform</param>
        /// <param name="index">需要清空到第几个子节点为止，默认为0，即全部清除</param>
        public static void KooDestroyChildrenIm(this GameObject target, int index = 0)
        {
            KooDestroyChildren(target?.transform, index);
        }

        #endregion

        #endregion

        #region AddComponent 添加组件(静态拓展)

        /// <summary>
        /// 获取或添加一个指定类型的组件到目标 GameObject 上。
        /// </summary>
        /// <typeparam name="T">要获取或添加的组件类型。</typeparam>
        /// <param name="obj">目标 GameObject。</param>
        /// <returns>获取或添加的组件实例。</returns>
        public static T KooGetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null)
            {
                return null;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }

        /// <summary>
        /// 获取或添加一个指定类型的组件到给定的 Component 所属的 GameObject 上。
        /// </summary>
        /// <typeparam name="T">要获取或添加的组件类型。</typeparam>
        /// <param name="target">给定的 Component。</param>
        /// <returns>获取或添加的组件实例。</returns>
        public static T KooGetOrAddComponent<T>(this Component target) where T : Component
        {
            return KooGetOrAddComponent<T>(target?.gameObject);
        }

        /// <summary>
        /// 获取或添加一个指定类型的组件到给定的 GameObject 上。
        /// </summary>
        /// <param name="target">给定的 GameObject。</param>
        /// <param name="type">要获取或添加的组件类型。</param>
        /// <returns>获取或添加的组件实例。</returns>
        public static Component KooGetOrAddComponent(this GameObject target, System.Type type)
        {
            if (target == null || type == null)
            {
                return null;
            }

            var component = target.GetComponent(type);
            if (component == null)
            {
                component = target.AddComponent(type);
            }

            return component;
        }

        /// <summary>
        /// 获取或添加一个指定类型的组件到给定的 GameObject 上。
        /// </summary>
        /// <param name="target">给定的 GameObject。</param>
        /// <param name="type">要获取或添加的组件类型。</param>
        /// <returns>获取或添加的组件实例。</returns>
        public static Component KooGetOrAddComponent(this Component target, System.Type type)
        {
            return KooGetOrAddComponent(target?.gameObject, type);
        }

        #endregion

        #region GetComponent 根据名称获取子物体组件

        /// <summary>
        /// 根据子物体名称获取其挂载的组件
        /// </summary>
        /// <param name="gameObject">游戏物体</param>
        /// <param name="searchName">子物体名称</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public static T KooFindComponentByChildName<T>(this GameObject gameObject, string searchName)
            where T : Component
        {
            var componentsInChildren = gameObject.GetComponentsInChildren<T>(true);
            var length = componentsInChildren.Length;
            for (var i = 0; i < length; i++)
            {
                var component = componentsInChildren[i];
                if (searchName == component.name)
                {
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据子物体名称获取其挂载的组件
        /// </summary>
        /// <param name="transform">GameObject的Transform</param>
        /// <param name="searchName">子物体名称</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public static T KooFindComponentByChildName<T>(this Transform transform, string searchName) where T : Component
        {
            var componentsInChildren = transform.GetComponentsInChildren<T>(true);
            var length = componentsInChildren.Length;
            for (var i = 0; i < length; i++)
            {
                var component = componentsInChildren[i];
                if (searchName == component.name)
                {
                    return component;
                }
            }

            return null;
        }

        #endregion

        #region GetChildCount 获取子节点数量

        /// <summary>
        /// 获取指定 Component 的子物体数量。
        /// </summary>
        /// <param name="target">指定的 Component。</param>
        /// <returns>子物体数量。</returns>
        public static int KooGetChildCount(this Component target)
        {
            if (target == null)
            {
                return 0;
            }

            return target.transform.childCount;
        }

        /// <summary>
        /// 获取指定 GameObject 的子物体数量。
        /// </summary>
        /// <param name="target">指定的 GameObject。</param>
        /// <returns>子物体数量。</returns>
        public static int KooGetChildCount(this GameObject target)
        {
            if (target == null)
            {
                return 0;
            }

            return target.transform.childCount;
        }

        #endregion

        #endregion

        #region 简化名称

        /// <summary>
        /// 完全简化名称
        /// </summary>
        /// <param name="prefab">要获取名称的预制体 GameObject 对象。</param>
        /// <returns>返回预制体的名称，去除 "(XXX)" 后缀。</returns>
        public static void KooCompleteSimplyPrefabName(this GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Prefab is null. 改名操作失败.");
                return;
            }

            string name = prefab.name;
            // 去除 "XXX)" 后缀并返回
            if (!name.EndsWith(")")) return;
            int lastIndex = name.LastIndexOf('(');
            if (lastIndex != -1 && name.EndsWith(")"))
            {
                //取值的最后一个字符索引为(在的位置-1
                int endIndex = lastIndex - 1;
                //如果有空格 再减1 指向名字的最后一个字符
                if (endIndex >= 0 && name[endIndex] == ' ')
                {
                    endIndex--;
                }

                string simplifiedName = name.Substring(0, endIndex + 1);
                prefab.name = simplifiedName;
            }
        }

        /// <summary>
        /// 获取简化的预制体名称。
        /// </summary>
        /// <param name="prefab">要获取名称的预制体 GameObject 对象。</param>
        /// <returns>返回预制体的名称，去除 "(Clone)" 后缀。</returns>
        public static void KooSimplyPrefabName(this GameObject prefab)
        {
            string name = prefab.transform.name;
            // 去除 "(Clone)" 后缀并返回
            if (name.EndsWith("(Clone)"))
            {
                name = name.Substring(0, name.Length - 7);
            }

            prefab.name = name;
        }

        /// <summary>
        /// 获取简化的预制体名称。
        /// </summary>
        /// <param name="prefab">要获取名称的预制体 GameObject 对象。</param>
        /// <returns>返回预制体的名称，去除 "(Clone)" 后缀。</returns>
        public static string KooGetSimplyPrefabName(this GameObject prefab)
        {
            string name = prefab.transform.name;
            // 去除 "(Clone)" 后缀并返回
            if (name.EndsWith("(Clone)"))
            {
                name = name.Substring(0, name.Length - 7);
            }

            return name;
        }


        /// <summary>
        /// 简化物品名称 简化一次 会去除名称最后一个 (XXX) 字符串
        /// </summary>
        /// <param name="prefab">要获取名称的预制体 GameObject 对象。</param>
        /// <returns>返回预制体的名称，去除 "(XXX)" 后缀。</returns>
        public static string KooGetCompleteSimplyName(this GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Prefab is null. 改名操作失败.");
                return "NULL";
            }

            string name = prefab.name;
            // 去除 "XXX)" 后缀并返回
            if (!name.EndsWith(")")) return prefab.name;
            int lastIndex = name.LastIndexOf('(');
            if (lastIndex != -1 && name.EndsWith(")"))
            {
                //取值的最后一个字符索引为(在的位置-1
                int endIndex = lastIndex - 1;
                //如果有空格 再减1 指向名字的最后一个字符
                if (endIndex >= 0 && name[endIndex] == ' ')
                {
                    endIndex--;
                }

                string simplifiedName = name.Substring(0, endIndex + 1);
                name = simplifiedName;
            }

            return name;
        }

        /// <summary>
        /// 获取去除 "(Clone)" 后缀的预制体 GameObject。
        /// 如果预制体的名称不包含 "(Clone)" 后缀，则返回原始预制体。
        /// 如果预制体的名称包含 "(Clone)" 后缀，则修改预制体名称，并返回修改后的预制体。
        /// </summary>
        /// <param name="prefab">要处理的预制体 GameObject 对象。</param>
        /// <returns>返回处理后的预制体 GameObject。</returns>
        public static GameObject GetSimplyNamePrefab(GameObject prefab)
        {
            string name = prefab.transform.name;

            // 如果名称包含 "(Clone)" 后缀，进行修改并返回
            if (name.Contains("(Clone)"))
            {
                name = name.Substring(0, name.Length - 7);
                prefab.transform.name = name;
            }

            return prefab;
        }

        /// <summary>
        /// 通过简化名称获取预制体，如果存在 "(Clone)" 后缀则移除。
        /// </summary>
        /// <param name="prefab">原始预制体 GameObject。</param>
        /// <returns>经过修改的预制体 GameObject。</returns>
        public static GameObject KooGetSimplyNamePrefab(this GameObject prefab)
        {
            string name = prefab.transform.name;

            // 如果名称包含 "(Clone)" 后缀，进行修改并返回
            if (name.Contains("(Clone)"))
            {
                name = name.Substring(0, name.Length - 7);
                prefab.transform.name = name;
            }

            return prefab;
        }

        #endregion
    }
}