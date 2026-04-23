using System;
using Unity.VisualScripting;

namespace interactive.Interface
{
    public interface IInteractable
    {
        /// <summary>
        /// 判断是否可以交互
        /// </summary>
        /// <returns></returns>
        bool CanInteract();
    }
}