function extractParamFromUri(uri, paramName) {
	if (!uri) {
		return;
	}
	var uri = uri.split('#')[0];
	var parts = uri.split('?');
	if (parts.length == 1) {
		return;
	}
	var query = decodeURI(parts[1]);

	paramName += '=';
	var params = query.split('&');
	for (var i = 0, param; param = params[i]; ++i) {
		if (param.indexOf(paramName) === 0) {
			return unescape(param.split('=')[1]);
		}
	}
}

var _gaq = _gaq || [];
_gaq.push(
	['_setAccount', 'UA-5032464-1'],
	['_setDomainName', '.barnamenevis.org'],
	['_trackPageview'],
	['_trackPageLoadTime']
);

(function() {
	var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
	ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
	var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();

YAHOO.util.Event.onDOMReady(function() { 

	setTimeout(function() {
		try {
			FB.Event.subscribe('edge.create', function(targetUrl) {
				_gaq.push(['_trackSocial', 'Facebook', 'Like', targetUrl]);
			});
			FB.Event.subscribe('edge.remove', function(targetUrl) {
				_gaq.push(['_trackSocial', 'Facebook', 'Unlike', targetUrl]);
			});
		} catch(e) {}

		try {
			twttr.events.bind('tweet', function(event) {
				if (event) {
					var targetUrl;
					if (event.target && event.target.nodeName == 'IFRAME') {
						targetUrl = extractParamFromUri(event.target.src, 'url');
					}
					_gaq.push(['_trackSocial', 'Twitter', 'Tweet', targetUrl]);
				}
			});
		} catch(e) {}
	}, 1000);
});