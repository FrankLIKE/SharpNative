   M S B A M L   `   `   `  ���� T  PMahApps.Metro, Version=0.9.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  I ECsNativeVisual, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:CsNativeVisualCsNativeVisual    CsNativeVisual.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Omahapps;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     ] YICSharpCode.AvalonEdit, Version=5.0.1.0, Culture=neutral, PublicKeyToken=9cc39be672370310?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  1csNativeVisualclr-namespace:CsNativeVisual    � Title$  CsNative Visual��$	��800��$
��1000�� � DataContext (  "CsNativeVisual.MainWindowViewModel   � 	Resources  �� MergedDictionaries �� Source �� 
TargetType �� Property �� Value   HeaderStyle �� Color  HeaderStyleFaded	 �� BasedOn��  �� $H Bpack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml?��� $F @pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml?��� $I Cpack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml?��� $M Gpack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml?��� $R Lpack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml?�%U  'Q�       &	  ,     &	      7   �� " Q�.���  �@Q�
IsExpanded . �� " ��.���  � ��$ Center=���  � ��$ 5,5,5,5q���  � ��$ Center=���  � ��$2 ,Segoe UI Light, Lucida Sans Unicode, Verdana)���  � ��$ 27(���  � �� �� $ Coral���� " ��8	   .���  � �� �� $
 Gray��.��� 
 � RowDefinitions
 �� $
��Auto ��� .+��� .+���  �� Orientation$ 
Horizontal=�.+��� -   $��4q�.���� $
��Open���� -   $��4q�.���� -    ��Name$ OpenSolution��$��Open Solution���� -   $��4q�.���� �� $�Options���� �� � $��1�� � ColumnDefinitions �� $��* ��� $��* ��� $��* �
 �� $��50 ��� $��* ��� $��50 ��� $	��150 �.+��� $��0��$��0��$ 
Horizontal=�.+��� #��[  $��C# Source Code���� -   $��RunCSharpButton��$��0q�$��30��
������ $��Right=�.���� $��Icons/run.png��$��Right=��� -    ��Name$ SaveButton_Test��$
��Save��$
��Left=�$��40��$��30��$��Bottom=�$��0,0,0,19.759q��� $��0��$��1��$��10,10.241,200.455,10.309q�
�������$��1q� �� Padding$ 5q�$��2��._�'  !ICSharpCode.AvalonEdit.TextEditor  -     Name$ 
TextEditor��$��Consolas)�   SyntaxHighlightingM  GICSharpCode.AvalonEdit.Highlighting.HighlightingDefinitionTypeConverter$ C#    WordWrap .    ShowLineNumbers . �� $��2��$��0��$ 
Horizontal=�$��106.545,-8,60.183,7.759q�.+��� #��[  $	��C++��$��5,5,5,4.759q��� -   $ RunCPPButton��$��0q�$��30��
������ $��Right=�.���� $��Icons/run.png��$��Right=��� $��1��$��1��$"��144.272,10.241,10.183,10.309q�
�������$��1q�$ 5q�$��2��._�  -	   $ Output��$��Consolas)�$	 C++  .  . � $��0��$��3��$��2��$��2�� �� $��* ��� $��* ��� $��* �
 �� $��40 ��� $��50 ��� $��* �.+��� $��0��$��0��$ 
Horizontal=�.+��� $��10,0,5,0q�$��19(�.��	Actions�� -
   $��
TestButton��$��10,0,5,0q�$
��Test��$��19(��� -   $��
TestStatus��$��10,0,5,0q�$��STATUS��$��19(��� $��0��$��3��$��1��#��[ $��Output���� $��0��$��3��$��2��.���� -    ��Name$ Errors�� �� TextWrapping$
 Wrap=� �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�$��10q�
�������$��1q� �� 
IsReadOnly . ���� *CompilerErrors+ �� Mode$ OneWay=��� -    ��Name$ CppFileList��$��1��$
��Left=�$��32��$��144.272,8,0,0q�$	��Top=�$	��267��$��0��$��2���� -   $ 
SaveButton��$
��Save��$��2��$
��Left=�$��276.545,10,0,0q�$	��Top=�$��45��$��30��