mergeInto(LibraryManager.library, {
	OnPositionDataReceived: function (_jsonString) {
		var posObj = JSON.parse(Pointer_stringify(_jsonString));
		if(posObj.Type > 2 || posObj.Type == 0)
		{
			console.log("Error: Invalid Type Parameter");
			return;
		}
		console.log("Pos X: " + posObj.Position.x + " Y: " + posObj.Position.y + " Z: " + posObj.Position.z);
		console.log("Rota X: " + posObj.Rotation.x + " Y: " + posObj.Rotation.y + " Z: " + posObj.Rotation.z);
	},
});