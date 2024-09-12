//****************** 代码文件申明 ************************
//* 文件：ShowIfEmptyAttribute                      
//* 作者：wheat
//* 创建时间：2023年09月22日 星期五 16:00
//* 描述：判断字段是否为空 为空就显示在界面上 不空就不显示在界面上
//*****************************************************



using System;
using Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ShowIfNullAttribute : ShowInInspectorAttribute
{
    public ShowIfNullAttribute() { }
}
