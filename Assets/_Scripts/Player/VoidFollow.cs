using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidFollow : MonoBehaviour
{
    private void Update() {
        transform.transform.position = Player.Instance.transform.position;
    }
}
