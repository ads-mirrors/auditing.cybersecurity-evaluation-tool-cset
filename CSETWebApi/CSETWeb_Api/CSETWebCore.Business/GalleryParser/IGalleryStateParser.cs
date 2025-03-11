//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;

namespace CSETWebCore.Business.GalleryParser
{
    public interface IGalleryState
    {
        GalleryBoardData GetGalleryBoard(string layout_name);

    }
}