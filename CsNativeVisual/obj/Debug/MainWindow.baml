   M S B A M L   `   `   `  ����T  PMahApps.Metro, Version=0.9.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5      I ECsNativeVisual, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:CsNativeVisualCsNativeVisual 5         CsNativeVisual.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Omahapps;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5      ] YICSharpCode.AvalonEdit, Version=5.0.1.0, Culture=neutral, PublicKeyToken=9cc39be672370310?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  5      1csNativeVisualclr-namespace:CsNativeVisual  5        � Title$  CsNative Visual��5      $	��800��5      $
��1000��6    � DataContext 5	      (  "CsNativeVisual.MainWindowViewModel  5
   
    � 	Resources 5       �� MergedDictionaries �� Source �� 
TargetType �� Property �� Value   HeaderStyle �� Color  HeaderStyleFaded	 �� BasedOn�� 5   
    5      �� 5      $H Bpack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml?�6%   �� 5      $F @pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml?�6%   �� 5      $I Cpack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml?�6%   �� 5      $M Gpack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml?�6%   �� 5      $R Lpack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml?�6%   %@  'Q�       &	  Q     &	 �    7   �� 5      " Q�6   .�5      ��  �@Q�
IsExpanded . 6/   5      �� 5      " ��6   .�5      ��  � ��$ Center=�6?   �� 5       � ��$ 5,5,5,5q�6+   �� 5       � ��$ Center=�6=   �� 5       � ��$2 ,Segoe UI Light, Lucida Sans Unicode, Verdana)�6/   �� 5       � ��$ 27(�6-   �� 5       � �� 5      �� 5       $ Coral��6*   5"      5#      �� 5$      " ��6   8	   .�5%      ��  � �� 5&      �� 5'      $
 Gray��6*   5)      5*      5+      .��5-      � 
 � RowDefinitions
 5.   
   �� 5/      $
��Auto �6   �� 50      .+�52   
   �� .+�53      ��  �� Orientation$ 
Horizontal=�6   .+�54      �� -   5        $��4q�55      .��57      �� $
��Open��6    58      �� 59      -   5        $��4q�5:      .��5<      �� -   5         ��Name$ OpenSolution��5<       $��Open Solution��66   5=      �� 5>      -   5        $��4q�5?      .��5A      �� �� 6"   $�Options��6&   �� 67   �� 6C   6J   5B      5C      5D      � 5E   
   $��1��6    � ColumnDefinitions 5F      �� 5G      $��* �6#   �� 5H      $��* �6#   �� 5I      $��* �6#   
 5K      �� 5L      $��50 �6    �� 5M      $��* �6    �� 5N      $��50 �6    �� 5O      $	��150 �6    .+�5Q      �� $��0��6   $��0��6)   $ 
Horizontal=�66   .+�5R      �� #��[  6   $��C# Source Code��6>   �� 5S      -   5        $��RunCSharpButton��5S   o   $��0q�6   $��30��6.   
������ $��Right=�6S   .��5T      �� $��Icons/run.png��6   $��Right=�63   5U      �� 5V      -   5         ��Name$ SaveButton_Test��5V      $
��Save��63   $
��Left=�6B   $��40��6]   $��30��6�   $��Bottom=�6�   $��0,0,0,19.759q�6�   5W      �� 5Y      $��0��6   $��1��6%   $��10,10.241,200.455,10.309q�62   
�������$��1q�6l    �� Padding$ 5q�6�   $��2��6�   ._�5[      '  !ICSharpCode.AvalonEdit.TextEditor  -   5          Name$ 
TextEditor��5[   (   $��Consolas)�5\         SyntaxHighlightingM  GICSharpCode.AvalonEdit.Highlighting.HighlightingDefinitionTypeConverter$ C# 6"      WordWrap .    ShowLineNumbers . 5]      5`      �� 5b      $��2��6   $��0��6)   $ 
Horizontal=�66   $��106.545,-8,60.183,7.759q�6O   .+�5d      �� #��[  6   $	��C++��6=   $��5,5,5,4.759q�6K   �� 5f      -   5        $ RunCPPButton��5f   o   $��0q�6   $��30��6.   
������ $��Right=�6S   .��5g      �� $��Icons/run.png��6   $��Right=�63   5h      5i      �� 5k      $��1��6   $��1��6%   $"��144.272,10.241,10.183,10.309q�62   
�������$��1q�6p   $ 5q�6�   $��2��6�   ._�5l        -	   5        $ Output��5l   (   $��Consolas)�5m      $	 C++ 6"    .  . 5n   #   5o      � 5q      $��0��6   $��3��6#   $��2��67   $��2��6D    5r      �� 5s      $��* �6'   �� 5t      $��* �6'   �� 5u      $��* �6'   
 5w      �� 5x      $��40 �6$   �� 5y      $��50 �6$   �� 5z      $��* �6$   .+�5}      �� $��0��6   $��0��6.   $ 
Horizontal=�6;   .+�5~      �� $��10,0,5,0q�6   $��19(�6.   .��6<   	Actions6E   �� 5      -
   5        $��
TestButton��5   N   $��10,0,5,0q�6   $
��Test��60   $��19(�6@   6`   �� 5�      -   5        $��
TestStatus��5�   Q   $��10,0,5,0q�6   $��STATUS��60   $��19(�6B   6Q   5�      �� 5�      $��0��6   $��3��6(   $��1��6<   #��[ 6I   $��Output��6s   �� 5�      $��0��6   $��3��6/   $��2��6C   .��5�      �� -   5         ��Name$ Errors��5�       �� TextWrapping$
 Wrap=�6]    �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�5�   
   $��10q�5�      
�������$��1q�6*    �� 
IsReadOnly . ��5�   /   �� *CompilerErrors+ �� Mode$ OneWay=�5�   >   5�      5�      �� 5�      -   5         ��Name$ CppFileList��5�      $��1��6+   $
��Left=�6;   $��32��6V   $��144.272,8,0,0q�6b   $	��Top=�6y   $	��267��6�   $��0��5�      $��2��6#   �� 5�      -   5        $ 
SaveButton��5�      $
��Save��6*   $��2��69   $
��Left=�6I   $��276.545,10,0,0q�6d   $	��Top=�6|   $��45��6�   $��30��6�   6�   5�      5�      5�      