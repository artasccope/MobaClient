LMgrBase = class()

function LMgrBase:ctor()
	self.eventTree = {}
	self.msgType = nil
end

function LMgrBase:SendMsg(lMsgBase)
	if lMsgBase ~= nil and lMsgBase.msgId ~= nil then
		if lMsgBase.msgType == self.msgType then
			self:ProcessEvent(lMsgBase)
		else
			--TODO
		end
	end
end


function LMgrBase:ProcessEvent(lMsgBase)
	if lMsgBase ~= nil and lMsgBase.msgId ~= nil then
		local msgId = lMsgBase.msgId
		local dic = self.eventTree[msgId]
		if dic ~= nil and #dic > 0 then
			for i,v in ipairs(dic) do
				v:ProcessEvent(lMsgBase)
			end
		else
			print("msg isn't existed in eventTree, msgId == "..lMsgBase.msgId..", MsgType == "..lMsgBase.msgType)
		end
	end
end




 