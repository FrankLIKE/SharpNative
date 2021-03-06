module System.Array_T;
import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;
import std.stdio;

class ArrayIterator(T):IEnumerator_T!(T)
	{
		int index = -1;
		Array_T!(T) _array;

		this(Array_T!(T) array)
		{
			_array = array;
			//Console.WriteLine(_S("inted with {0}"), BOX!(int)(index));
			//writeln(_array.Items[index]);
		}

		public T  IEnumerator_T_Current()   @property
		{
			//writeln(_array is null);
			//Console.WriteLine(_S("returning {0}"), BOX!(int)(index));
			//writeln(_array.Items[index]);

			return _array.Items[index];
			//return cast(T)null;
		}

		void IDisposable_Dispose()
		{
			_array = null;
		}


		bool IEnumerator_MoveNext()
		{
			index++;
			if(index < _array.Length)
				return true;
			return false;
		}

		NObject IEnumerator_Current() @property
		{
			return BOX!(T)(IEnumerator_T_Current); // BOX should be adjusted to just pass classes as is
		}

		void IEnumerator_Reset()
		{
			index = -1;
		}

	}

/*
 Array_T!(int[]) fiver = new Array_T!(int[])([ [1,2],[8],[3],[4],[6] ],2,3,4);
      
      Console.WriteLine(fiver.Ranks);
*/
public class Array_T(T=NObject) :  NObject, ICollection_T!(T) 
//if(!is(T:void))
{
	private int index_;

	private T[] _items;
	private int[] _dims;

	T[] Items() @property
	{
		return _items;
	}

	int Rank() @property
	{
		return cast(int)_dims.length;
	}

	int GetLength(int dimension=0)
	{

		if(Rank==1)
		{
			if(dimension==0)
				return cast(int)_items.length;
			else
				throw new IndexOutOfRangeException();
		}
		else if(Rank > dimension)
			return _dims[dimension];
		else
			throw new IndexOutOfRangeException();

	}

	int GetUpperBound(int dimension=0)
	{

		if(Rank==1)
		{
			if(dimension==0)
				return cast(int)_items.length-1;
			else
				throw new IndexOutOfRangeException();
		}
		else if(Rank > dimension)
			return _dims[dimension]-1;
		else
			throw new IndexOutOfRangeException();

	}


	int GetLowerBound(int dimension=0)
	{
		//we can add support later, but its really an unoptimization
		return 0;
		//if(Rank==1)
		//{
		//	if(dimension==0)
		//		return cast(int)_items.length-1;
		//	else
		//		throw new IndexOutOfRangeException();
		//}
		//else if(Rank > dimension)
		//	return _dims[rank]-1;
		//else
		//	throw new IndexOutOfRangeException();

	}

	 void Items(T[] newItems) @property
	{
		 _items = newItems;
	}
	//params and array params are treated the same ... so int[] and 1,2,3,... are similar
	this(__IA!(T[]) ac,int[] dims...)
	{
		auto array = ac.A;
		//Console.WriteLine("initing array...");
		//Console.WriteLine(array);
		if(dims is null)
		{
			if(array !is null)
			{
				dims = [cast(int)array.length];
			}
		}
		_dims = dims;
		_items = array;
		_iter = new ArrayIterator!(T)(this);
	}



	this(int[] dims...)
	{
		_dims = dims;

		//Console.WriteLine(_dims);

		int totaldims = 1;

		for(int i=0;i<_dims.length;i++)
			totaldims*= _dims[i];
		
		//if(totaldims==1)
		//_items =  T[];
		//else
		_items = new T[totaldims];


		_iter = new ArrayIterator!(T)(this);
	}
	
	public	int Length() @property 
	{
		return cast(int) _items.length;
	}

	public void Reverse()
	{
		_items.reverse;
	}

	
	
	public T GetValue(int index)
	{
		return _items[index];
	}

	public void SetValue(T value,int index)
	{
		 _items[index] = value;
	}

	public void CopyTo(Array_T!(T) other,int start=0,int end=-1)
	{
		if(end==-1)
			other.Items = _items[start..$].dup;
		else
			other.Items = _items[start..end+1].dup;

	 //data.CopyTo(temp.data, 0);
	}



	//Adds foreach support
//	Foreach Range Properties
//		Property	Purpose
//			.empty	returns true if no more elements
//				.front	return the leftmost element of the range
//					Foreach Range Methods
//					Method	Purpose
//					.popFront()	move the left edge of the range right by one


		final bool empty() {
			bool result = (index_ == _items.length);
			if (result)
				index_ = 0;
			return result;
		}
		
		final void popFront() {
			if (index_ < _items.length)
				index_++;
		}
		
		final T front() {
			return _items[index_];
		}

//Specialized for PInvoke and other uses
		final U opCast(U)() if(is(U:char**))// && is(T:string))
		{
			//throw new Exception("Sibitegeera");
			//exit(0);
			//copy with real addresses so the array can be modified
			char[][] charArray = new char[][Items.length];

			foreach(elem; Items)
			{
			//	Console.WriteLine(elem);
				charArray = charArray ~ cast(char[])(cast(string)elem);
			}

			return cast(U) charArray;
		}


		final U opCast(U)() if(is(U:T*))// && is(T:string))
		{
			////throw new Exception("Sibitegeera");
			////exit(0);
			////copy with real addresses so the array can be modified
			//char[][] charArray = new char[][Items.length];

			//foreach(elem; Items)
			//{
			////	Console.WriteLine(elem);
			//	charArray = charArray ~ cast(char[])(cast(string)elem);
			//}

			return cast(U) Items;
		}

//Needs fix, look at MatrixTest.cs
	final  void opIndexAssign(T value, int[] index...)
	{
		//Console.WriteLine("Assigning ...");
		int[] _indices = index; // .dup is slew

		auto finalindex = 0;
		auto len =cast(int)_indices.length;

		
		//Optimize common scenarios, slight performance boosts ... Add others
		
		 if(index.length==2) 
		{
			finalindex = _indices[0] * _dims[1]  + _indices[1];
		//Console.WriteLine("Assigning 2d...:" ~ std.conv.to!string(finalindex));


		}
		else
		 if(index.length==3) 
		{
			finalindex = _indices[0] * _dims[1] *_dims[2] + _indices[1] * _dims[2] + _indices[2];
		////Console.WriteLine("Assigning 3d...:" ~ std.conv.to!string(finalindex));

		}
		else
		{
		for(int i=len-1;i>=0;i--)
		{
			int multiplier = _indices[i];
			for(int j=i;j<len-1;j++)
			{
				multiplier*= _dims[j+1];
			}
			finalindex += multiplier;
		}
		}
	
		//Console.WriteLine("Assigning: "~ std.conv.to!string(value) ~ " to: " ~ std.conv.to!string(finalindex));


		_items[finalindex] =value;
		
	}



	final  void opIndexAssign(T value, int index)  {
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		_items[index] = value;
	}
	
	final  ref T opIndex(int index) { //TODO: ref could be a bad idea 
		//but allows alot of natural c# syntax
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		return _items[index];
	}

//Needs fix, look at MatrixTest.cs
	final  ref T opIndex(int[] index...) {
		//Console.WriteLine("Assigning ...");
		int[] _indices = index; // .dup is slew

		auto finalindex = 0;
		auto len =cast(int)_dims.length;

	//Optimize common scenarios, slight performance boosts ... Add others
		
		 if(len==2) 
		{
			finalindex = _indices[0] * _dims[1]  + _indices[1];
		//Console.WriteLine("Assigning 2d...:" ~ std.conv.to!string(finalindex));


		}
		else
		 if(len==3) 
		{
			finalindex = _indices[0] * _dims[1] *_dims[2] + _indices[1] * _dims[2] + _indices[2];
		////Console.WriteLine("Assigning 3d...:" ~ std.conv.to!string(finalindex));

		}
		else
		{
		for(int i=len-1;i>=0;i--)
		{
			int multiplier = _indices[i];
			for(int j=i;j<len-1;j++)
			{
				multiplier*= _dims[j+1];
			}
			finalindex += multiplier;
		}
		}

	

		//Console.WriteLine("Returning:" ~ std.conv.to!string(finalindex));
		//Console.WriteLine("Returning: "~ std.conv.to!string(_items[finalindex]) ~ " from: " ~ std.conv.to!string(finalindex));


		return _items[finalindex];
		
	}


	ArrayIterator!T _iter;

	//IEnumerator Methods
	IEnumerator  IEnumerable_GetEnumerator()
	{
		if(_iter is null)
			_iter = new ArrayIterator!(T)(this);

		_iter.IEnumerator_Reset();

		return _iter;
		//return new ArrayIterator!(T)(this); //Highly inefficient
		
	}

	IEnumerator_T!(T) IEnumerable_T_GetEnumerator()
	{
			if(_iter is null)
			_iter = new ArrayIterator!(T)(this);

		_iter.IEnumerator_Reset();
		
		return _iter;
		//return new ArrayIterator!(T)(this); //Highly inefficient
		//throw new NotSupportedException();
	}



	//ICollection Methods
	void ICollection_T_Add(T item)
	{
		throw new NotSupportedException();
	}

	public  bool  ICollection_T_IsReadOnly() @property
	{
		throw new NotSupportedException();
	}


	bool ICollection_T_Remove(T item)
	{
		throw new NotSupportedException();
	}

	bool ICollection_T_Contains(T item)
	{
		throw new NotSupportedException();
	}

	public void ICollection_T_CopyTo(Array_T!(T) array, int arrayIndex)
	{
		throw new NotSupportedException();
	}


	void ICollection_T_Clear()
	{
		throw new NotSupportedException();
	}

	int ICollection_T_Count() @property
	{
		return cast(int)_items.length;
	}

	int opApply(int delegate(ref T) action)
	{
		int r;
			
			for (auto i = 0; i < _items.length; i++) {
				if ((r = action(_items[i])) != 0)
					break;
			}
			
			return r;
	}

}

