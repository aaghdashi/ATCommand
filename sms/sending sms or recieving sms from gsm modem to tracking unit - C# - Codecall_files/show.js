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
		loadScript('//ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js',init);
	} else {
		init();
	}
	function init() {
		var $j = window.jQuery.noConflict();
		if($j('#binpress-store-widget').length == 1) {
			$j('#binpress-store-widget').attr('id','binpress-product-widget');
		}
		var el = $j('#binpress-product-widget');
		var color = el.attr('color');
		color = color || 'red';
		loadStylesheet(url + 'styles/widget/main-' + color+ '.css');
		var params = {};
		
		var attr = {component:'id',ad:'ad',language:'language',platform:'platform',framework:'framework',u:'ad'};
		for(var key in attr) {
			if(el.attr(key) != '') {
				params[attr[key]] = el.attr(key);
			}
		}
		
		$j.getJSON(url + 'widget/get?callback=?', params,function(response) {
			if(typeof(response.html) != 'undefined') {
				el.html(response.html);
			}
		});
	}
})();