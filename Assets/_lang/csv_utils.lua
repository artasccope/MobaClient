--csv表解析

local comma = ","--分隔符

local function split(str, reps, rest)
    for match in (str..reps):gmatch("(.-)"..reps) do
        table.insert(rest, match)
    end
end

local function split_line(str)  
    -- print(str)
    local reps = '\n\r'
    local resultStrsList = {}
    --[^,]+   查找非reps字符，并且多次匹配
    string.gsub(str, '[^'..reps..']+', function(w) table.insert(resultStrsList, w) end )
    return resultStrsList
end

local function split_content(str, reps)  
    -- print(str)
    local resultStrsList = {}

    --匹配出双引号
    local count = 0
    local s_index = 1
    local e_index = 1

    local i = 1  
    while true do  
        local index = string.find(str, "\"", i)  --匹配引号内容
        if not index then break end  

        count = count + 1  
        if count % 2 == 0 then
            --中间为字符串内容
            e_index = index
            local sub = string.sub(str, s_index + 1, e_index - 1)
            table.insert(resultStrsList, sub)
        else
            s_index = index
            if e_index ~= s_index then--非引号中的内容有分隔符分割
                local sub = string.sub(str, e_index, s_index - 2)
                split(sub, reps, resultStrsList)
            end
        end
        i = index + 1  
    end  

    if e_index ~= 1 then
        local last = string.sub(str, e_index + 1)
        if last ~= "" and last ~= nil then
            split(last, reps, resultStrsList)
        end
    else--中间没有引号
        split(str, reps, resultStrsList)
    end
    
    return resultStrsList
end

local function parse_csv_file(data)   
    -- 按行划分  
    local lines = split_line(data)
    --[[  
    从第4行开始保存（第一行是字段名，第二行是类型，第三行是注释，后面的行才是内容）   
    用二维数组保存：arr[ID字符串][属性标题字符串]  
    --]]  
    local titles = split_content(string.sub(lines[1], 4), comma)--字段
    local types = split_content(lines[2], comma)--类型

    local idType = types[1]
    local isNum = (idType == "int")

    local count = #titles
    local colType--列字段类型
    print("#titles count : "..#titles)

    local rest = {}--result table
    for i = 4, #lines, 1 do  --从第4行开始为表内容，第3行为列名
        local str = lines[i]
        if str == nil then break end

        local content = split_content(str, comma) --一行中，每一列的内容 
        -- 以标题作为索引，保存每一列的内容，取值的时候这样取：rest[1].Title  
        local id = (isNum and tonumber(content[1])) or content[1] --ID
        local t = {}
        for j = 1, count, 1 do  
            
            colType = types[j]
            local value = content[j]
            local key = titles[j]
            if colType == 'int' then
                t[key] = tonumber(value)
            else
                t[key] = value
            end

        end 
        rest[id] = t-- print(id)
    end  
    return rest
end  

function parse_csv_to_table(filePath)   
    -- 读取文件  
    local addr = io.open(filePath, "rb")
    buffer = addr:read "*a"
    addr:close()

    -- print(buffer)

    return parse_csv_file(buffer)
end
print("csv_utils")