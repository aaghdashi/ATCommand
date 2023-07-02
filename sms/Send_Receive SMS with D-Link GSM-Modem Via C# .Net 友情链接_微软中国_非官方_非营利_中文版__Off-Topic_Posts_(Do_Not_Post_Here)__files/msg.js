var downMsg=function(msgid,contentid,config){
	this.msg = $i(msgid);
	this.content = $i(contentid);
	this.config = config ? config : {start_delay:3000, speed: 5, movepx:2,cookie:'downMsgcookie',expiresDay:0};
	this.offsetHeight;
	this.emsg_objTimer;
	this.ie6Add =0;
	var _this = this;
	
	this.init = function(){
		var ifcookie=_jsc.cookies.getCookie(this.config.cookie);
		if(ifcookie == "show")
			return;
		window.setTimeout(_this.start,parseInt(_this.config.start_delay,10));
	}
	
	this.start = function(){
		_this.msg.style.display="block";
		_this.content.style.display="block";
		_this.offsetHeight = _this.content.offsetHeight;
		_this.content.style.height ="0px";
		_this.emsg_objTimer = setInterval(_this.moveUpDiv,parseInt(_this.config.speed,10));
	}
	
	this.moveUpDiv = function(){
	  if(_this.offsetHeight> parseInt(_this.content.style.height,10)){
	  	_this.content.style.height =  parseInt(_this.content.style.height,10)+parseInt(_this.config.movepx,10)+"px";
	  }
	  else{
	  	window.clearInterval(_this.emsg_objTimer);
	  	_jsc.cookies.setCookie(_this.config.cookie,"show",_this.config.expiresDay);
	  	// ie6下才做，因为没有fixed属性
	  	var isMSIE = !!(/*@cc_on!@*/0);
		if(isMSIE &&!(window.XMLHttpRequest))
		{ 
	  	_this.content.style.height = parseInt(_this.content.style.height,10) +2+"px";
	  	_this.autoMoveIe6();
	  	}
	  }
	}
	this.autoMoveIe6 = function(){

		if(_this.ie6Add ==0){
			_this.content.style.height =  parseInt(_this.content.style.height,10) + 1 +"px";
			_this.msg.style.bottom="-2px";
			_this.ie6Add =1;
		}
		else{
			_this.content.style.height =  parseInt(_this.content.style.height,10) - 1 +"px";
			_this.msg.style.bottom="-1px";
			_this.ie6Add =0;
		}
		setTimeout(_this.autoMoveIe6,100)
	}
}

function closeDiv()
{
	document.getElementById('downmsg_emessage').style.display='none';
}

function showHideDiv()
{
	var ct = document.getElementById('donwmsg_content');
	var btn = document.getElementById('msg_hidden_btn');
	if(ct.style.display!="none"){
	  ct.style.display = "none"
	  btn.className="msg-hidden-btn-2";
	}else{
	  ct.style.display="block";
	  btn.className="msg-hidden-btn-1";
	}

}