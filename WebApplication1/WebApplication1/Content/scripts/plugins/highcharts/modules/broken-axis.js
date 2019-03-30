(function(i){function q(){return Array.prototype.slice.call(arguments,1)}var o=i.pick,n=i.wrap,r=i.extend,p=HighchartsAdapter.fireEvent,k=i.Axis,s=i.Series;r(k.prototype,{isInBreak:function(e,h){var c=e.repeat||Infinity,b=e.from,a=e.to-e.from,c=h>=b?(h-b)%c:c-(b-h)%c;return e.inclusive?c<=a:c<a&&c!==0},isInAnyBreak:function(e,h){var c=this.options.breaks,b=c&&c.length,a,g,d;if(b){for(;b--;)this.isInBreak(c[b],e)&&(a=!0,g||(g=o(c[b].showPoints,this.isXAxis?!1:!0)));d=a&&h?a&&!g:a}return d}});n(k.prototype,
"setTickPositions",function(e){e.apply(this,Array.prototype.slice.call(arguments,1));if(this.options.breaks){var h=this.tickPositions,c=this.tickPositions.info,b=[],a;if(!(c&&c.totalRange>=this.closestPointRange)){for(a=0;a<h.length;a++)this.isInAnyBreak(h[a])||b.push(h[a]);this.tickPositions=b;this.tickPositions.info=c}}});n(k.prototype,"init",function(e,h,c){if(c.breaks&&c.breaks.length)c.ordinal=!1;e.call(this,h,c);if(this.options.breaks){var b=this;b.doPostTranslate=!0;this.val2lin=function(a){var g=
a,d,c;for(c=0;c<b.breakArray.length;c++)if(d=b.breakArray[c],d.to<=a)g-=d.len;else if(d.from>=a)break;else if(b.isInBreak(d,a)){g-=a-d.from;break}return g};this.lin2val=function(a){var g,d;for(d=0;d<b.breakArray.length;d++)if(g=b.breakArray[d],g.from>=a)break;else g.to<a?a+=g.len:b.isInBreak(g,a)&&(a+=g.len);return a};this.setExtremes=function(a,b,d,c,h){for(;this.isInAnyBreak(a);)a-=this.closestPointRange;for(;this.isInAnyBreak(b);)b-=this.closestPointRange;k.prototype.setExtremes.call(this,a,b,
d,c,h)};this.setAxisTranslation=function(a){k.prototype.setAxisTranslation.call(this,a);var c=b.options.breaks,a=[],d=[],h=0,e,f,l=b.userMin||b.min,m=b.userMax||b.max,j,i;for(i in c)f=c[i],e=f.repeat||Infinity,b.isInBreak(f,l)&&(l+=f.to%e-l%e),b.isInBreak(f,m)&&(m-=m%e-f.from%e);for(i in c){f=c[i];j=f.from;for(e=f.repeat||Infinity;j-e>l;)j-=e;for(;j<l;)j+=e;for(;j<m;j+=e)a.push({value:j,move:"in"}),a.push({value:j+(f.to-f.from),move:"out",size:f.breakSize})}a.sort(function(a,b){return a.value===b.value?
(a.move==="in"?0:1)-(b.move==="in"?0:1):a.value-b.value});c=0;j=l;for(i in a){f=a[i];c+=f.move==="in"?1:-1;if(c===1&&f.move==="in")j=f.value;c===0&&(d.push({from:j,to:f.value,len:f.value-j-(f.size||0)}),h+=f.value-j-(f.size||0))}b.breakArray=d;p(b,"afterBreaks");b.transA*=(m-b.min)/(m-l-h);b.min=l;b.max=m}}});n(s.prototype,"generatePoints",function(e){e.apply(this,q(arguments));var h=this.xAxis,c=this.yAxis,b=this.points,a,g=b.length,d=this.options.connectNulls,i;if(h&&c&&(h.options.breaks||c.options.breaks))for(;g--;)if(a=
b[g],i=a.y===null&&d===!1,!i&&(h.isInAnyBreak(a.x,!0)||c.isInAnyBreak(a.y,!0)))b.splice(g,1),this.data[g]&&this.data[g].destroyElements()});n(i.seriesTypes.column.prototype,"drawPoints",function(e){e.apply(this);var e=this.points,h=this.yAxis,c=h.breakArray||[],b=o(this.options.threshold,h.min),a,g,d,i,k,f;for(i=0;i<e.length;i++){g=e[i];f=g.stackY||g.y;for(k=0;k<c.length;k++){d=c[k];a=!1;if(b<d.from&&f>d.to||b>d.from&&f<d.from)a="pointBreak";else if(b<d.from&&f>d.from&&f<d.to||b>d.from&&f>d.to&&f<
d.from)a="pointInBreak";a&&p(h,a,{point:g,brk:d})}}})})(Highcharts);
