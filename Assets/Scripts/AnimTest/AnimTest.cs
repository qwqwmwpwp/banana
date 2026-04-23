using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[System.Serializable]
public class SlotVisibilityItem
{
    public string slotName;
    public bool visible;

    [System.NonSerialized] public Attachment originalAttachment;
}


[RequireComponent(typeof(SkeletonMecanim))]
public class AnimTest : MonoBehaviour
{
    public List<SlotVisibilityItem> slotVisibilityList = new List<SlotVisibilityItem>();

    private SkeletonMecanim skeletonMecanim;
    private Skeleton skeleton;

    void Start()
    {
        skeletonMecanim = GetComponent<SkeletonMecanim>();
        skeleton = skeletonMecanim.Skeleton;

        RefreshSlotList();
        
        ApplySlotVisibility();
    }

    void Update()
    {
        ApplySlotVisibility();
    }


    public void RefreshSlotList()
    {
        slotVisibilityList.Clear();

        foreach (var slot in skeleton.Slots)
        {
            slotVisibilityList.Add(new SlotVisibilityItem
            {
                slotName = slot.Data.Name,
                visible = true, // 預設全部顯示
                originalAttachment = slot.Attachment,
            });
        }

        slotVisibilityList.Sort((a, b) => string.Compare(a.slotName, b.slotName));
    }

    public void ApplySlotVisibility()
    {
        foreach (var item in slotVisibilityList)
        {
            var slot = skeleton.FindSlot(item.slotName);
            if (slot == null) continue;
    
            if (item.visible)
            {
                // 若原始 attachment 存在，恢復顯示
                if (item.originalAttachment != null)
                    slot.Attachment = item.originalAttachment;
            }
            else
            {
                slot.Attachment = null;
            }
        }

    }
}
