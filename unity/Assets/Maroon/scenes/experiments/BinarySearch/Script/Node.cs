using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.CSE.BinarySearchTree {
    public class Node : MonoBehaviour {
        public int value;
        public Node parentNode;
        public Node lowNode;
        public Node highNode;
        public Transform tf;
        private Text text_value;
        public bool isRoot;
        public Vector3 rootPosition;
        void Start() {
            text_value = GetComponent<Text>();
            text_value.text = value.ToString();
            tf = GetComponent<Transform>();
        }

        void Update() {
            if (isRoot) {
                tf.position = rootPosition;
            }
            if (lowNode) {
                lowNode.tf.position = tf.position - tf.up*15 - tf.right*25;
            }
            if (highNode) {
                highNode.tf.position = tf.position + tf.right*25 - tf.up*15;
            }
        }
    }
}
