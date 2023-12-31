// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: PB_Person.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from PB_Person.proto</summary>
public static partial class PBPersonReflection {

  #region Descriptor
  /// <summary>File descriptor for PB_Person.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PBPersonReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg9QQl9QZXJzb24ucHJvdG8iPQoJUEJfUGVyc29uEgwKBG5hbWUYASABKAkS",
          "CwoDYWdlGAIgASgFEhUKDXBob25lX251bWJlcnMYAyADKAliBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PB_Person), global::PB_Person.Parser, new[]{ "Name", "Age", "PhoneNumbers" }, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PB_Person : pb::IMessage<PB_Person> {
  private static readonly pb::MessageParser<PB_Person> _parser = new pb::MessageParser<PB_Person>(() => new PB_Person());
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<PB_Person> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PBPersonReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PB_Person() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PB_Person(PB_Person other) : this() {
    name_ = other.name_;
    age_ = other.age_;
    phoneNumbers_ = other.phoneNumbers_.Clone();
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PB_Person Clone() {
    return new PB_Person(this);
  }

  /// <summary>Field number for the "name" field.</summary>
  public const int NameFieldNumber = 1;
  private string name_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Name {
    get { return name_; }
    set {
      name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "age" field.</summary>
  public const int AgeFieldNumber = 2;
  private int age_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Age {
    get { return age_; }
    set {
      age_ = value;
    }
  }

  /// <summary>Field number for the "phone_numbers" field.</summary>
  public const int PhoneNumbersFieldNumber = 3;
  private static readonly pb::FieldCodec<string> _repeated_phoneNumbers_codec
      = pb::FieldCodec.ForString(26);
  private readonly pbc::RepeatedField<string> phoneNumbers_ = new pbc::RepeatedField<string>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<string> PhoneNumbers {
    get { return phoneNumbers_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as PB_Person);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(PB_Person other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Name != other.Name) return false;
    if (Age != other.Age) return false;
    if(!phoneNumbers_.Equals(other.phoneNumbers_)) return false;
    return true;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Name.Length != 0) hash ^= Name.GetHashCode();
    if (Age != 0) hash ^= Age.GetHashCode();
    hash ^= phoneNumbers_.GetHashCode();
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Name.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Name);
    }
    if (Age != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Age);
    }
    phoneNumbers_.WriteTo(output, _repeated_phoneNumbers_codec);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Name.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
    }
    if (Age != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Age);
    }
    size += phoneNumbers_.CalculateSize(_repeated_phoneNumbers_codec);
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(PB_Person other) {
    if (other == null) {
      return;
    }
    if (other.Name.Length != 0) {
      Name = other.Name;
    }
    if (other.Age != 0) {
      Age = other.Age;
    }
    phoneNumbers_.Add(other.phoneNumbers_);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          input.SkipLastField();
          break;
        case 10: {
          Name = input.ReadString();
          break;
        }
        case 16: {
          Age = input.ReadInt32();
          break;
        }
        case 26: {
          phoneNumbers_.AddEntriesFrom(input, _repeated_phoneNumbers_codec);
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
