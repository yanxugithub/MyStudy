select gc.groupcontrolid, gc.childcontrolid, LEVEL, SYS_CONNECT_BY_PATH(gc.groupcontrolid, '/') "Path"
from pf_groupcontrol gc left join pf_groupcontrol gc1 on gc.groupcontrolid = gc1.childcontrolid 
where gc.childcontrolid is not null
connect by  gc.groupcontrolid  = prior gc.childcontrolid 
ORDER SIBLINGS BY gc.groupcontrolid, gc.childcontrolid
