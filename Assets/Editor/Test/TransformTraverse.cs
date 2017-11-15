using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
    public class TransformTraverse
    {
        [MenuItem("Algorithm/Traverse preorder recursively")]
        public static void TraverseTransformRecursivelyPreOrder() {
            GameObject go = Selection.gameObjects[0];
            TraverseTransformRecursivelyPreOrder(go.transform);
        }

        public static void TraverseTransformRecursivelyPreOrder(Transform transform) {
            Debug.Log(transform.name);
            for (int i = 0; i < transform.childCount; i++) {
                TraverseTransformRecursivelyPreOrder(transform.GetChild(i));
            }
        }

        [MenuItem("Algorithm/Traverse preorder no recursively")]
        public static void TraverseTransformNoRecursivelyPreOrder() {
            GameObject go = Selection.gameObjects[0];
            TraverseTransformNoRecursivelyPreOrder(go.transform);
        }

        public static void TraverseTransformNoRecursivelyPreOrder(Transform transform) {
            Stack<Transform> transStack = new Stack<Transform>();
            transStack.Push(transform);

            while (transStack.Count > 0) {
                Transform curTrans = transStack.Pop();
                Debug.Log(curTrans.name);

                for (int i = curTrans.childCount - 1; i > -1; i--) {
                    transStack.Push(curTrans.GetChild(i));
                }
            }

            
        }
    }
}
