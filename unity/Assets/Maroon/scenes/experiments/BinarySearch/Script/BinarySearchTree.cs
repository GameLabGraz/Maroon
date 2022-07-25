using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.CSE.BinarySearchTree {
    public class BinarySearchTree : MonoBehaviour {
        public Maroon.CSE.BinarySearchTree.Node root;
        public Maroon.CSE.BinarySearchTree.Node nodePrefab;
        private Maroon.CSE.BinarySearchTree.Node currentNode;
        public InputField input_field;

        void Start() {}

        void Update() {
            // if (Input.GetKeyDown(KeyCode.Space)) {
            //     insert(Random.Range(0,99));
            // }
        }

        public void insert(int val) {
            val = int.Parse(input_field.text);
            Debug.Log(val);
            Maroon.CSE.BinarySearchTree.Node newNode = Instantiate(nodePrefab); //cloning nodePrefab to newNode to specify the cloned object's position and rotation
            newNode.gameObject.transform.SetParent(GameObject.Find("root").transform); //determine the position, rotation and scale of object in the scene
            newNode.value = val;
            bool added = false;
            if (!root) {
                root = newNode;
                newNode.isRoot = true;
            }
            else {
                currentNode = root;
                while (!added) {
                    if (newNode.value > currentNode.value) {
                        if (currentNode.highNode) {
                            currentNode = currentNode.highNode;
                        } else {
                            currentNode.highNode = newNode;
                            newNode.parentNode = currentNode;
                            added = true;
                        }
                    } else if (newNode.value < currentNode.value) {
                        if (currentNode.lowNode) {
                            currentNode = currentNode.lowNode;
                        } else {
                            currentNode.lowNode = newNode;
                            newNode.parentNode = currentNode;
                            added = true;
                        }
                    } else {
                        //value == root => not add
                        added = true;
                        Destroy (newNode.gameObject);
                    }
                }
            }
        }

        public Maroon.CSE.BinarySearchTree.Node search(int val) {
            val = int.Parse(input_field.text);
            if (root) {
                currentNode = root;
                if (val == root.value) {
                    return root;
                }
                while (currentNode.value != val) {
                    if (val < currentNode.value) {
                        if (currentNode.lowNode) {
                            currentNode = currentNode.lowNode;
                        }
                        else  {break;}
                    }
                    else if (val > currentNode.value) {
                        if (currentNode.highNode) {
                            currentNode = currentNode.highNode;
                        }
                        else {break;}
                    }
                }
                if (currentNode.value == val) {
                    return currentNode;
                }
            }
            return null;
        }
    }
}
