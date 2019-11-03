// Misc.js

String.prototype.right = function(len)
{
	len = this.length - len;
	return this.substr(len >= 0 ? len : 0);
}