require "class"
require "define"

LMgrBase = class()
function LMsgBase:ctor(msgId, msgMap)
	self.msgId = msgId
	self.msgType = math.floor(msgId /MsgSpan)
end






