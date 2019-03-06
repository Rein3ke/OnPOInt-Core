let poiData = [];
let sceneData = [];

let poiName;
let poiDescription;

window.onload = function () {
    poiName         = $("#poi_name");
    poiDescription  = $("#poi_description");

    getJsonFromUrl('scene_lib.json', function (_wasSuccessful, _data) {
        if (_wasSuccessful) {
            sceneData = _data;
            initializeSceneSelection();
            getJsonFromUrl('poi_lib.json', function (_wasSuccessful, _data) {
                if (_wasSuccessful) {
                    poiData = _data;
                }
            });
        }
    });
};

function onPoiIdReceived(_id) {
    console.log(`${_id} received from Unity!`);
    if(_id === -1)
    {
        setToDefault();
        return;
    }

    let poiPoint = poiData.filter(_o => _o.ID === _id)[0];
    console.log(poiPoint);

    if (poiPoint === undefined) {
        console.log(`POI ${_id} not found! Please hug a cat.`);
        setToDefault();
        return;
    }

    poiName.text(poiPoint.Name);
    poiDescription.text(poiPoint.Description);
}

function setToDefault() {
    poiName.text("None");
    poiDescription.text("Please look at a POI point.");
}

function getJsonFromUrl(url, _onResponse) {
    $.getJSON(url, function (data) {
        if (data === undefined) {
            _onResponse(false, undefined);
            return;
        }
        _onResponse(true, data['Data']);
    })
}

function initializeSceneSelection() {
    $.each(sceneData, function (_key, _val) {
        $('#sceneSelection').append(`<option value="${_val['ID']}">${_val['Name']}</option>`);
    })
}

function sendSceneIDToUnity(_id) {
    console.log("Send to Unity: " + _id);
    gameInstance.SendMessage('GameManager', 'ReceiveSceneID', Number(_id));
}

function ToggleLockState(_isLocked) {
    let lockState; // A number representing the lock state in Unity (0 = NONE, 1 = LOCKED, 2 = CONFINED)

    /**
     * Ueberprueft den gesetzen LockState und setzt, wenn true, den GameContainer in HTML auf fokussiert.
     */
    if (_isLocked) {
        document.getElementById("gameContainer").requestPointerLock();
        //document.getElementById("gameContainer").focus();
        lockState = 1;
    } else {
        lockState = 0;
        //document.getElementById("gameContainer").blur();
    }

    // Sendet eine Request an Unity zum Setzen des internen LockState.
    console.log("Send to Unity: " + lockState);
    gameInstance.SendMessage('GameManager', 'SetLockState', lockState);
}
