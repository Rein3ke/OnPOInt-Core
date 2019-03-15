mergeInto(LibraryManager.library, {
	OnDataReceived: function (_jsonString) {
		var pObject = JSON.parse(Pointer_stringify(_jsonString));
		switch(pObject.Type)
		{
			default: //NONE
				console.log("Error: Invalid type parameter provided.");
			break;
			case 1: //POI
				console.log("pObject ID: " + pObject.ID);
				onPoiIdReceived(pObject.ID);
			break;
			case 2: //SCENE_CHANGE
				console.log("pObject ID: " + pObject.ID);
				
			break;
		}
	},
});