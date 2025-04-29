import { FaRegBell } from 'react-icons/fa';
import {Badge} from 'antd';


export default function Notification() {
    return(<>
    <Badge count={5} size="small" className="text-red-100 cursor-pointer" >
        <FaRegBell className="w-5 h-5 text-[#EBECF0]"/>
    </Badge>
    </>)
}