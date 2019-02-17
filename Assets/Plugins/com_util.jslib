mergeInto(LibraryManager.library, {
	OnDataReceived: function (_jsonString) {
		var pObject = JSON.parse(Pointer_stringify(_jsonString));
		switch(pObject.Type)
		{
			default: //NONE
				console.log("Error: Invalid type parameter provided.");
			break;
			case 1: //POI
				if(pObject.ID === -1)
				{
					$("#poi_name").text("None");
					$("#poi_description").text("Please look at a POI point.");
					return;
				}
				$("#poi_name").text(pObject.Name);
				$("#poi_description").text(pObject.Description);
				//console.log("ID: " + pObject.ID);
				//console.log("Name: " + pObject.Name);
				//console.log("Description: " + pObject.Description);
			break;
			case 2: //POS_UPDATE
				console.log("Pos X: " + pObject.Position.x + " Y: " + pObject.Position.y + " Z: " + pObject.Position.z);
				console.log("Rota X: " + pObject.Rotation.x + " Y: " + pObject.Rotation.y + " Z: " + pObject.Rotation.z);
			break;
			case 3: //SPEED_CHANGE
			
			break;
			case 4: //SCENE_CHANGE
			
			break;
		}
	},
});