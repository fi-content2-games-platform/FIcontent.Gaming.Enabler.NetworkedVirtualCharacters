
var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

( function() {
    "use strict";

    var _fivesCommunicator = FIVES.Communication.FivesCommunicator;

    var nvc = function () {
        FIVES.Events.AddConnectionEstablishedHandler(this._createFunctionWrappers.bind(this));
        FIVES.Events.AddOnComponentUpdatedHandler(this._componentUpdatedHandler.bind(this));
        //FIVES.Events.AddEntityGeometryCreatedHandler(this._handleEntityGeometryCreated.bind(this));
    };

    var m = nvc.prototype;

    m._createFunctionWrappers = function() {
        this.createDragon = _fivesCommunicator.connection.generateFuncWrapper("NVC.createDragon");
        this.updateBones = _fivesCommunicator.connection.generateFuncWrapper("NVC.updateBones");
    };

    //m._handleEntityGeometryCreated = function(entity) {
        // var $defElement = $(entity.xml3dView.defElement);
        // entity.xml3dView.nvcElements = {};
        // entity.xml3dView.nvcElements.boneRotations = $defElement.find("float4[id*='_rotation']");
        // entity.xml3dView.nvcElements.boneTranslations = $defElement.find("float3[id*='_translation']");
    //};

    m._componentUpdatedHandler = function(entity, componentName, attributeName) {
        if(componentName != "skeleton")
			return;

		//console.log("skeleton update");
		return;
		
		// TODO: 
		var numElems = entity.skeleton.translations.length;
		var trans = new Array(numElems * 3);
		var rot = new Array(numElems * 4);
		
		for (var i = 0; i < numElems; i++) {
			var tidx = 3*i;
			var ct = entity.skeleton.translations[i];
			trans[tidx  ] = ct.x;
			trans[tidx+1] = ct.y;
			trans[tidx+2] = ct.z;

			var ridx = 4*i;
			var cr = entity.skeleton.rotations[i];
			rot[ridx  ] = cr.x;
			rot[ridx+1] = cr.y;
			rot[ridx+2] = cr.z;
			rot[ridx+3] = cr.w;
		}
		
		// entity.xml3dView.nvcElements.boneTranslations.setScriptValue(trans);
		entity.xml3dView.nvcElements.boneTranslations.text(trans.join(" "));
		entity.xml3dView.nvcElements.boneRotations.text(rot.join(" "));
		// this._applyTranslationKeyframeUpdates(entity);
        // this._applyOrientationKeyframeUpdates(entity);
    };

    // m._applyTranslationKeyframeUpdates = function(entity) {
        // entity.xml3dView.mplanElements.translationKeys.text(entity["mplanAnimation"]["translationKeyframes"].join(" "));
    // };

    // m._applyOrientationKeyframeUpdates = function(entity) {
        // entity.xml3dView.mplanElements.rotationKeys.text(entity["mplanAnimation"]["orientationKeyframes"].join(" "));
    // };

    FIVES.Plugins.NVC = new nvc();
}());

