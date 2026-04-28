using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace qwq
{
    public class Bullet : MonoBehaviour
    {
        public ElementType enmu = ElementType.water;
        public float v = 10f;
        public Vector2 direction;
        public Rigidbody2D rb {  get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Destroy(gameObject, 3f);
            rb.velocity = (Vector3)direction * v;
        }

        private void FixedUpdate()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.GetComponent<IInteraction>()?.Trigger(gameObject);
            Debug.Log(collision.name);
        }
    }



    public interface IInteraction
    {
        public void Trigger(GameObject gObj);
    }

    public enum ElementType
    {
        water,
        ice
    }

}