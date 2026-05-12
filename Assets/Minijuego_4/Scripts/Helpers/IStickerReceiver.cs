using UnityEngine;

public interface IStickerReceiver
{
    bool CanAcceptSticker(StickerType type);
}