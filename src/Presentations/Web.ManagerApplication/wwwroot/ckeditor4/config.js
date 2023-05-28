/**
 * @license Copyright (c) 2003-2022, CKSource Holding sp. z o.o. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	config.fullPage= true;
	config.allowedContent = true;
	config.filebrowserBrowseUrl = '/Elfinder/browse';
	config.extraPlugins = 'lineheight,docprops';//  bootstrapTabs //bật cái này và trong file js là xong
	//config.line_height = "10px;15px;18px;20px;25px;30px;35px";
	//config.contentsCss = 'https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css';
	//config.fontSize_sizes= '12px;2.3em;130%;larger;x-small';
};
//CKEDITOR.on('instanceReady', function (ev) {
//	var jqScript = document.createElement('script');
//	var bsScript = document.createElement('script');

//	jqScript.src = 'https://code.jquery.com/jquery-2.0.2.js';
//	bsScript.src = 'https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js';

//	var editorHead = ev.editor.document.$.head;
//	editorHead.appendChild(jqScript);
//	editorHead.appendChild(bsScript);
//	//setTimeout(function () {
		
//	//}, 2000)
	
//});


// Load CK Editor
//CKEDITOR.replace('editor1', {
//	contentsCss: 'http://netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css'
//});