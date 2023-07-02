;(function(){
	var url = 'http://www.binpress.com/';
	function loadScript(src,callback) {
		var head= document.getElementsByTagName('head')[0];
		var script= document.createElement('script');
		script.type= 'text/javascript';
		
		if(typeof(callback) == 'function') {
			script.onload = function() { 
				if ( ! script.onloadDone ) {
					script.onloadDone = true; 
					callback.apply(this);
				}
			};
			script.onreadystatechange = function() { 
				if ( ( "loaded" === script.readyState || "complete" === script.readyState ) && ! script.onloadDone ) {
					script.onloadDone = true; 
					callback.apply(this);
				}
			};
		}
		script.src= src;
		head.appendChild(script);
	}
	function loadStylesheet(src) {
		var head = document.getElementsByTagName("head")[0];         
		var cssNode = document.createElement('link');
		cssNode.type = 'text/css';
		cssNode.rel = 'stylesheet';
		cssNode.href = src;
		cssNode.media = 'screen';
		head.appendChild(cssNode);
	}
	if(!window.jQuery) {
		loadScript('//ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js',init);
	} else {
		init();
	}
	function init() {
		var $j = window.jQuery.noConflict();
		var el = $j('#binpress-store-listing');
		
		loadStylesheet(url + 'styles/widget/store.css');
		var params = {};
		
		var attr = ['language','framework','platform','ids','category','u','limit'];
		for(var i in attr) {
			if(el.attr(attr[i]) != '' && typeof(el.attr(attr[i])) != 'undefined') {
				params[attr[i]] = el.attr(attr[i]);
			}
		}
		var request = function(params) {
			if(el.find('.components').length == 1) {
				var comp = el.find('.components');
				var height = Math.round(comp.height() / 2);
				var width = comp.width();
				el.find('.components').html('<div style="width:' + width + 'px;height:' + height + 'px;padding-top:' + height + 'px;" class="binpress-loader"><img src="' + url + 'images/big-loading.gif" /></div>');
			}
			$j.getJSON(url + 'widget/storerender?callback=?', params,function(response) {
				if(typeof(response.html) != 'undefined') {
					el.html(response.html);
					events();
				}
			});
		}
		request(params);
		
		var events = function() {
			el.find('.pagination a').click(function(e){
				e.preventDefault();
				var p = params;
				p['page'] = $j(this).attr('name');
				request(p);
			});
			el.find('.sorting ul li a').click(function(e){
				e.preventDefault();
				var p = params;
				p['order'] = $j(this).attr('rel');
				if(p['page']) {
					delete(p['page']);
				}
				request(p);
			});
		}
	}
	

})();