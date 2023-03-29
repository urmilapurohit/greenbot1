/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For complete reference see:
	// https://ckeditor.com/docs/ckeditor4/latest/api/CKEDITOR_config.html

	// The toolbar groups arrangement, optimized for a single toolbar row.
	config.toolbarGroups = [
		{ name: 'document',	   groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'clipboard',   groups: [ 'clipboard', 'undo' ] },
		{ name: 'editing',     groups: [ 'find', 'selection', 'spellchecker' ] },
		{ name: 'forms' },
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph',   groups: [ 'list', 'indent', 'blocks', 'align', 'bidi' ] },
		{ name: 'links' },
		{ name: 'insert' },
		{ name: 'styles' },
		{ name: 'colors' },
		{ name: 'tools' },
		{ name: 'others' },
		{ name: 'about' }
	];

	// The default plugins included in the basic setup define some buttons that
	// are not needed in a basic editor. They are removed here.
	config.removeButtons = 'Cut,Copy,Paste,Undo,Redo,Anchor,Underline,Strike,Subscript,Superscript';

	// Dialog windows are also simplified.
    config.removeDialogTabs = 'link:advanced';
    //config.format_h1 = { element: 'h1', attributes: { 'class': 'editorTitle1' } };
    //config.format_h2 = { element: 'h2', attributes: { 'class': 'editorTitle2' } };
    config.removePlugins = 'easyimage, cloudservices,image';
    config.format_tags = 'p;h1;h2;h3;h4;h5;h6';
    //config.extraAllowedContent = 'a[id](tagged)';
    config.extraAllowedContent = 'a[data-emailid](tagged);a[data-userid];img[src,alt,width,height]';
    //config.extraAllowedContent = 'img[src,alt,width,height]';
    //config.extraAllowedContent = 'a(tagged)';
    //config.extraAllowedContent = 'a(*)';
   
    config.mentions = [{
        minChars: 1,
        outputTemplate: `<a href="#" class="tagged" data-emailid="{email}" data-userid="{userId}">{name}</a>`,
        feed: function (options, callback) {
            debugger;
            var xhr = new XMLHttpRequest();
            var userid = urlUserID;
            var UserTypeId = userTypeId;
            var jobId = jobID;
            xhr.onreadystatechange = function () {

                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {
                        callback(JSON.parse(this.responseText));
                    } else {
                        callback([]);
                    }
                }
            }

            xhr.open('GET', '/Job/GetUserList?name=' + encodeURIComponent(options.query) + "&userid=" + userid + "&userTypeId=" + UserTypeId + "&JobId=" + jobId);
            xhr.send();
        },

    }];
};
